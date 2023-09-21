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

    void Start()
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
        // Move the tower first
        currentTower.transform.position = newPosition;

        // Then check for validity
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
        if (!gameManager.AllTowers.Contains(shootScript))
        {
            gameManager.AllTowers.Add(shootScript);
            gameManager.money -= shootScript.cost;
        }
        currentTowerSpriteRenderer.color = Color.white;
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
                // Double click detected
                Tower towerScript = hitCollider.gameObject.GetComponent<Tower>();
                if (towerScript != null)
                {
                    gameManager.RemoveTower(towerScript); // Remove from the GameManager list
                }
                Destroy(hitCollider.gameObject); // Destroy the object
            }
            else
            {
                // Single click detected, update the lastClickTime
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

        Debug.Log("Checking cell: " + cellPosition);
        Debug.Log("World Location: " + location);

        if (!groundTilemap.HasTile(cellPosition))
        {
            Debug.Log("Invalid: No ground tile at " + cellPosition);
            return false;
        }

        // Gets the tower's collider
        Collider2D towerCollider = currentTower.GetComponent<Collider2D>();

        // Creates an array to store results. Let's assume no more than 10 for now.
        Collider2D[] results = new Collider2D[10];

        // Checks for overlaps
        int numResults = towerCollider.OverlapCollider(new ContactFilter2D(), results);

        // Loops through results to see if any are tagged as "PathTilemap"
        for (int i = 0; i < numResults; i++)
        {
            if (results[i].gameObject.CompareTag("PathTilemap"))
            {
                Debug.Log("Invalid: Overlaps with path.");
                return false;
            }
        }

        Debug.Log("Valid placement at " + cellPosition);
        return true;
    }
}
