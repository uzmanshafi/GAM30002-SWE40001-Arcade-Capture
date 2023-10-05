using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public GameObject radiusPrefab;
    private GameObject currentTower;
    private GameObject currentRadius;

    private UIManager uiManager;
    private SpriteRenderer currentTowerSpriteRenderer;
    private GameManager gameManager;
    private float scaleFactor = 0.5f;
    private float towerPlacedCooldown = 0.5f;

    //using these variables for star rating ui expection and animation
    private GameObject starRatingUI;
    private Vector3 originalStarRatingPos;
    private float moveDuration = 1.5f;
    private bool starRatingIsUp = false;
    private bool isMovingStarRating = false;

    //control indicators
    [SerializeField] private GameObject Controlindicator;

    void Start()
    {
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;

        starRatingUI = GameObject.FindGameObjectWithTag("starRating");
        Debug.Log("found" + " " + starRatingUI);

        if (starRatingUI != null)
        {
            originalStarRatingPos = starRatingUI.transform.position;
        }


    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        PlacementCooldown();
        CancelTowerPlacement();
        TowerDragging(mouseWorldPos);
        TowerRotation();
        RunTowerPlacement(mouseWorldPos);
        TowerSelection(mouseWorldPos);
    }

    private void PlacementCooldown()
    {
        if (towerPlacedCooldown > 0)
        {
            towerPlacedCooldown -= Time.deltaTime;
        }
    }

    private void CancelTowerPlacement()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
        {
            DestroyCurrentTower();
        }
    }

    private void TowerDragging(Vector3 mouseWorldPos)
    {
        if (currentTower != null && towerPlacedCooldown <= 0)
        {
            DragTower(mouseWorldPos);
        }
    }

    private void TowerRotation()
    {
        if (currentTower != null && (Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y != 0))
        {
            currentTower.transform.Rotate(0, 0, -45);
        }
    }

    private void RunTowerPlacement(Vector3 mouseWorldPos)
    {
        if (currentTower != null && Input.GetMouseButtonDown(0) && IsValidLocation(mouseWorldPos, currentTower.name))
        {
            DropTower();
            towerPlacedCooldown = 0.5f; // Reset cooldown after placing a tower
        }
    }

    private void TowerSelection(Vector3 mouseWorldPos)
    {
        if (Input.GetMouseButtonDown(0) && currentTower == null && towerPlacedCooldown <= 0)
        {
            AttemptSelectTower(mouseWorldPos);
        }
    }

    private void DestroyCurrentTower()
    {
        if (currentTower != null)
        {
            if(currentTower.TryGetComponent<PongTower>(out PongTower pt))
            {
                Destroy(pt.other.gameObject);
            }
            Destroy(currentTower);
            Destroy(currentRadius);
            currentTower = null;
            currentTowerSpriteRenderer = null;
        }

        Controlindicator.SetActive(false);
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


        uiManager.deselectTower();
        DestroyCurrentRadius();

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
        else if (currentTower.TryGetComponent<PongTower>(out pt) && Vector3.Distance(currentTower.transform.position, currentRadius.transform.position) > pt.range)
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
        else
        {
            currentTowerSpriteRenderer.color = new Color(0, 1, 0, 0.8f);

        }

        if (currentRadius)
        {
            if (currentTower.TryGetComponent<PongTower>(out pt) && pt.other != null)
            {

            }
            else
            {
                currentRadius.transform.position = newPosition;
            }
        }

        // Checks the distance to the starRatingUI
        float distanceToStarRating = Vector3.Distance(currentTower.transform.position, starRatingUI.transform.position);
        float moveThreshold = 2f;

        if (distanceToStarRating < moveThreshold && !starRatingIsUp)
        {
            MoveStarRatingUp();
        }
        else if (distanceToStarRating >= moveThreshold && starRatingIsUp)
        {
            MoveStarRatingToOriginalPos();
        }

        Controlindicator.SetActive(true); // displays the control indicator
    }
    private void DropTower()
    {
        MoveStarRatingToOriginalPos();
        Tower shootScript = currentTower.GetComponent<Tower>();

        if (currentTower.TryGetComponent<PongTower>(out PongTower pt) && pt.other == null)
        {
            
            currentTowerSpriteRenderer.color = Color.white;

            currentTower = Instantiate(towerPrefabs[5]);
            currentTower.transform.position = GetMouseWorldPosition(); //+ new Vector3(1.5f, 1.5f, 0); idk what this is supposed to do but is causing bugs
            currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();

            pt.other = currentTower.GetComponent<PongTower>();
            pt.TowerOrder = 0;
            pt.other.other = pt;

            currentTower.GetComponent<Tower>().enabled = false;
            shootScript.enabled = true;
        }
        else if (currentTower.TryGetComponent<PongTower>(out PongTower pt2) && pt2.other != null)
        {
            if (Vector2.Distance(pt2.transform.position, pt2.other.transform.position) <= pt2.other.range)
            {
                
                shootScript.enabled = true;
                if (!gameManager.AllTowers.Contains(shootScript))
                {
                    gameManager.AllTowers.Add(shootScript);
                    gameManager.money -= shootScript.cost;
                }
                pt2.TowerOrder = 1;
                currentTowerSpriteRenderer.color = Color.white;
                currentTower = null;
                currentTowerSpriteRenderer = null;
                DestroyCurrentRadius();
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
            DestroyCurrentRadius();
        }

        Controlindicator.SetActive(false);
    }


    private void AttemptSelectTower(Vector3 position)
    {

        int layerMask = 1 << LayerMask.NameToLayer("Tower");
        Collider2D hitCollider = Physics2D.OverlapPoint(position, layerMask);

        Rect uiRect = uiManager.GetComponent<RectTransform>().rect;

        if (hitCollider != null && hitCollider.gameObject.CompareTag("ArcadeTower"))
        {
            Tower towerScript = hitCollider.gameObject.GetComponent<Tower>();
            if (towerScript != null)
            {
                uiManager.selectTower(towerScript);
                ShowRadiusForSelectedTower(towerScript);
            }
        }
        else if (uiRect.Contains(position))
        {

        }
        else
        {
            uiManager.deselectTower();
            DestroyCurrentRadius();
        }

    }

    private void ShowRadiusForSelectedTower(Tower tower)
    {
        DestroyCurrentRadius();

        currentRadius = Instantiate(radiusPrefab, tower.transform.position, Quaternion.identity);
        currentRadius.transform.SetParent(tower.transform); // Set the tower as the parent of the radius
        UpdateRadiusDisplay(tower.range);
    }


    private void UpdateRadiusDisplay(float range)
    {
        float desiredRadius = range * scaleFactor;
        currentRadius.transform.localScale = new Vector2(desiredRadius, desiredRadius);
    }

    public void DestroyCurrentRadius()
    {
        if (currentRadius)
        {
            Destroy(currentRadius);
            currentRadius = null;
        }

        
    }

    //star rating related function are here
    private void MoveStarRatingUp()
    {
        if (!starRatingIsUp && !isMovingStarRating) // Check if coroutine is not already running
        {
            StopAllCoroutines(); // Stops any existing move coroutines
            StartCoroutine(MoveStarRating(originalStarRatingPos, new Vector3(originalStarRatingPos.x, originalStarRatingPos.y + 1, originalStarRatingPos.z)));
            starRatingIsUp = true;
        }
    }

    private void MoveStarRatingToOriginalPos()
    {
        if (starRatingIsUp && !isMovingStarRating) // Checks if coroutine is not already running
        {
            StopAllCoroutines(); // Stops any existing move coroutines
            StartCoroutine(MoveStarRating(starRatingUI.transform.position, originalStarRatingPos));
            starRatingIsUp = false;
        }
    }


    private IEnumerator MoveStarRating(Vector3 startPos, Vector3 endPos)
    {
        if (isMovingStarRating) yield break; // Checks if coroutine is already running
        isMovingStarRating = true;

        float journeyLength = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;
        float distanceCovered;

        do
        {
            distanceCovered = (Time.time - startTime) * moveDuration;
            float fractionOfJourney = distanceCovered / journeyLength;
            starRatingUI.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

            yield return null; // Waits for next frame
        }
        while (distanceCovered < journeyLength);

        starRatingUI.transform.position = endPos; // Ensures the final position is set correctly
        isMovingStarRating = false;
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

        Collider2D towerCollider = currentTower.GetComponent<Collider2D>();

        Collider2D[] results = new Collider2D[10];

        int numResults = towerCollider.OverlapCollider(new ContactFilter2D(), results);

        for (int i = 0; i < numResults; i++)
        {
            if (results[i].gameObject.CompareTag("PathTilemap"))
            {
                return false;
            }
        }

        int uiLayerMask = 1 << LayerMask.NameToLayer("UI");
        Collider2D hitCollider = Physics2D.OverlapPoint(location, uiLayerMask);

        if (hitCollider != null)
        {
            // Checks if the hit UI element is a child of starRatingUI
            if (hitCollider.transform.IsChildOf(starRatingUI.transform))
            {
                // if we are close to Star Rating UI, it will move up
                Debug.Log("Detected proximity to StarBar. Moving it up.");
                MoveStarRatingUp();
            }
            else
            {
                // if mouse is over some other UI element, so the location is not valid.
                return false;
            }
        }

        return true;
    }


}
