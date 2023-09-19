using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    private GameObject currentTower;
    private SpriteRenderer currentTowerSpriteRenderer;
    private float lastClickTime = 0;
    private float catchTime = 0.25f; // Max time in seconds between double clicks

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        // Destroy the tower when the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentTower != null)
            {
                Destroy(currentTower);
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
                RotateTower();
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
        currentTower = Instantiate(towerPrefab);
        currentTower.transform.position = position + new Vector3(1.5f, 1.5f, 0);
        currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
        Tower shootScript = currentTower.GetComponent<Tower>();
        shootScript.enabled = false;
    }

    private void DragTower(Vector3 newPosition)
    {
        currentTower.transform.position = newPosition;

        if (IsValidLocation(newPosition, currentTower.name))
        {
            currentTowerSpriteRenderer.color = new Color(0, 1, 0, 0.8f);
        }
        else
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
    }

    private void DropTower()
    {
        Tower shootScript = currentTower.GetComponent<Tower>();
        shootScript.enabled = true;
        currentTowerSpriteRenderer.color = Color.white;
        if (gameManager.money - shootScript.cost < 0)
        {
            Debug.Log("Cannnot afford");
            Destroy(currentTower);
        }
        else
        {
            if (!gameManager.AllTowers.Contains(shootScript))
            {
                gameManager.AllTowers.Add(shootScript);
            }
            gameManager.money -= shootScript.cost;
        }
        currentTower = null;
        currentTowerSpriteRenderer = null;
        
    }

    private void AttemptPickupTower(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(position);

        if (hitCollider != null && hitCollider.gameObject.CompareTag("ArcadeTower"))
        {
            if (Time.time - lastClickTime < catchTime)
            {
                
                if (hitCollider.gameObject.TryGetComponent<Tower>(out Tower t))
                {
                    gameManager.money += (int)(t.cost * 0.75f);
                    gameManager.AllTowers.Remove(t);
                }
                Destroy(hitCollider.gameObject);
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }

    private void RotateTower()
    {
        if (currentTower != null)
        {
            currentTower.transform.Rotate(0, 0, -45);
        }
    }

    private bool IsValidLocation(Vector3 location, string towerName)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(location);

        if (!groundTilemap.HasTile(cellPosition)) return false;
        if (pathTilemap.HasTile(cellPosition)) return false;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(location, 0.35f, 1 << LayerMask.NameToLayer("Tower"));
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != currentTower)
            {
                return false;
            }
        }

        return true;
    }
}
