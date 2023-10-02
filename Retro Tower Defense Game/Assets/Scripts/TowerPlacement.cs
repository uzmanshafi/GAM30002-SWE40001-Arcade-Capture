using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public Tilemap wallTilemap;
    public Collider2D wallCollider;

    public GameObject radiusPrefab;
    private GameObject currentTower;
    private GameObject currentRadius;
    private SpriteRenderer currentTowerSpriteRenderer;
    private float lastClickTime = 0;
    private float catchTime = 0.25f;
    GameManager gameManager;
    private float scaleFactor = 0.5f;

    [SerializeField] private bool keepTowerRadiusOn = false; //used to keep radius on when placing (just for testing remove in final build)

    void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentTower != null)
            {
                Destroy(currentTower);
                Destroy(currentRadius);
                currentTower = null;
                currentTowerSpriteRenderer = null;
            }
        }
        if (currentTower != null)
        {
            DragTower(mouseWorldPos);
            if (Input.GetMouseButtonDown(0))
            {
                if (IsValidLocation(mouseWorldPos, currentTower.name))
                {
                    DropTower();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                currentTower.transform.Rotate(0, 0, -45);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            AttemptPickupTower(mouseWorldPos);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        return mouseWorldPos;
    }

    public void SpawnTowerOnButtonClick(int index)
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        SpawnTower(towerPrefabs[index], mouseWorldPos);
    }

    private void SpawnTower(GameObject towerPrefab, Vector3 position)
    {
        if (gameManager.money - towerPrefab.GetComponent<Tower>().cost < 0)
        {
            return;
        }

        currentTower = Instantiate(towerPrefab);
        currentTower.transform.position = position + new Vector3(1.5f, 1.5f, 0);
        currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
        Tower shootScript = currentTower.GetComponent<Tower>();
        shootScript.enabled = false;

        currentRadius = Instantiate(radiusPrefab, currentTower.transform);
        UpdateRadiusDisplay(shootScript.range);
    }

    private void DragTower(Vector3 newPosition)
    {
        currentTower.transform.position = newPosition;

        PongTower pt;
        if (!IsValidLocation(newPosition, currentTower.name)) //swapped to add extra check for pong tower, is now implicitly red else its green
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
        else if(currentTower.TryGetComponent<PongTower>(out pt) && Vector3.Distance(currentTower.transform.position, currentRadius.transform.position) > pt.range)
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
        else
        {
            currentTowerSpriteRenderer.color = new Color(0, 1, 0, 0.8f);
            
        }

        if (currentRadius && (currentTower.TryGetComponent<PongTower>(out pt) && pt.other == null))
        {
            currentRadius.transform.position = newPosition;
        }
    }

    private void DropTower()
    {
        Tower shootScript = currentTower.GetComponent<Tower>();
        if (currentTower.TryGetComponent<PongTower>(out PongTower pt) && pt.other == null)
        {
            if (!gameManager.AllTowers.Contains(shootScript))
            {
                gameManager.AllTowers.Add(shootScript);
                gameManager.money -= shootScript.cost;
            }
            shootScript.enabled = true;
            currentTowerSpriteRenderer.color = Color.white;

            currentTower = Instantiate(towerPrefabs[5]);
            currentTower.transform.position = GetMouseWorldPosition() + new Vector3(1.5f, 1.5f, 0);
            currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();

            pt.other = currentTower.GetComponent<PongTower>();
            pt.TowerOrder = 0;
            pt.other.other = pt;

            currentTower.GetComponent<Tower>().enabled = false;
        }
        else if (currentTower.TryGetComponent<PongTower>(out PongTower pt2) && pt2.other != null)
        {
            if (Vector2.Distance(pt2.transform.position, pt2.other.transform.position) <= pt2.other.range)
            {
                shootScript.enabled = true;
                pt2.TowerOrder = 1;
                currentTowerSpriteRenderer.color = Color.white;
                currentTower = null;
                currentTowerSpriteRenderer = null;
                if (!keepTowerRadiusOn && currentRadius)
                {
                    Destroy(currentRadius);
                    currentRadius = null;
                }
            }
        }
        else
        {
            if (!gameManager.AllTowers.Contains(shootScript))
            {
                gameManager.AllTowers.Add(shootScript);
                gameManager.money -= shootScript.cost;
            }
            shootScript.enabled = true;
            currentTowerSpriteRenderer.color = Color.white;
            currentTower = null;
            currentTowerSpriteRenderer = null;
            if (!keepTowerRadiusOn && currentRadius)
            {
                Destroy(currentRadius);
                currentRadius = null;
            }
        }
    }


    private void AttemptPickupTower(Vector3 position)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Tower");
        Collider2D hitCollider = Physics2D.OverlapPoint(position, layerMask);
        if (hitCollider != null && hitCollider.gameObject.CompareTag("ArcadeTower"))
        {
            if (Time.time - lastClickTime < catchTime)
            {
                Tower towerScript = hitCollider.gameObject.GetComponent<Tower>();
                if (towerScript != null)
                {
                    gameManager.RemoveTower(towerScript);
                }
                gameManager.money += (int)(towerScript.cost * 0.80f);
                Destroy(hitCollider.gameObject);
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    private void UpdateRadiusDisplay(float range)
    {
        float desiredRadius = range * scaleFactor;
        currentRadius.transform.localScale = new Vector2(desiredRadius, desiredRadius);
    }


    private bool IsValidLocation(Vector3 location, string towerName)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(location);


        if (!groundTilemap.HasTile(cellPosition))
        {
            return false;
        }


        if (pathTilemap.HasTile(cellPosition))
        {
            return false;
        }


        Collider2D[] overlaps = Physics2D.OverlapBoxAll(location, currentTowerSpriteRenderer.bounds.size, 0);
        foreach (var overlap in overlaps)
        {
            if (overlap.gameObject.CompareTag("Wall"))
            {
                return false;
            }
        }

        // Additional check for SpaceInvadersTower due to its larger BoxCollider2D
        if (towerName.Equals("SpaceInvadersTower"))
        {
            Collider2D[] towerOverlaps = Physics2D.OverlapBoxAll(location, currentTower.GetComponent<BoxCollider2D>().size, 0);
            foreach (var overlap in towerOverlaps)
            {
                if (overlap.gameObject.CompareTag("Wall") || overlap.gameObject.CompareTag("Path"))
                {
                    return false;
                }
            }
        }

        // Gets the tower's collider
        Collider2D towerCollider = currentTower.GetComponent<Collider2D>();

        // Creates an array to store results
        Collider2D[] results = new Collider2D[10];

        // Checks for overlaps
        int numResults = towerCollider.OverlapCollider(new ContactFilter2D(), results);

        // Loops through results to see if any are tagged as "PathTilemap"
        for (int i = 0; i < numResults; i++)
        {
            if (results[i].gameObject.CompareTag("PathTilemap"))
            {
                return false;
            }
        }

        return true;
    }


}
