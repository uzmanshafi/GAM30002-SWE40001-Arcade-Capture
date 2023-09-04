using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject arcadeTowerPrefab;
    public GameObject spaceInvadersTowerPrefab;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    private GameObject currentTower;
    private SpriteRenderer currentTowerSpriteRenderer;

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnTower(arcadeTowerPrefab, mouseWorldPos);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnTower(spaceInvadersTowerPrefab, mouseWorldPos);
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

    private void SpawnTower(GameObject towerPrefab, Vector3 position)
    {
        if (currentTower == null)
        {
            Debug.Log("Attempting to spawn tower at: " + position);
            currentTower = Instantiate(towerPrefab);
            currentTower.transform.position = position + new Vector3(1.5f, 1.5f, 0);
            currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();

            
            SpaceInvadersTower spaceInvadersTowerScript = currentTower.GetComponent<SpaceInvadersTower>();
            if (spaceInvadersTowerScript != null)
            {
                spaceInvadersTowerScript.SetBoundaries(); 
            }
        }
    }


    private void DragTower(Vector3 newPosition)
    {
        currentTower.transform.position = newPosition;
        if (IsValidLocation(newPosition, currentTower.name))
        {
            currentTowerSpriteRenderer.color = new Color(0, 1, 0, 0.8f);  // Green with 10% opacity
        }
        else
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);  // Red with 10% opacity
        }
    }

    private void DropTower()
    {
        currentTowerSpriteRenderer.color = Color.white;  // Resets color to white
        currentTower = null;
        currentTowerSpriteRenderer = null;
    }

    private void AttemptPickupTower(Vector3 position)
    {
        Debug.Log("Attempt to pick up tower");

        Collider2D hitCollider = Physics2D.OverlapPoint(position);

        if (hitCollider != null)
        {
            Debug.Log("Collider hit: " + hitCollider.gameObject.name);

            if (hitCollider.gameObject.CompareTag("ArcadeTower"))
            {
                Debug.Log("Destroying and recreating tower");

                Destroy(hitCollider.gameObject);

                // Spawn a new tower and set it to currentTower so that you can drag it.
                currentTower = Instantiate(arcadeTowerPrefab, position, Quaternion.identity);
                currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void RotateTower()
    {
        if (currentTower != null)
        {
            currentTower.transform.Rotate(0, 0, 45);
        }
    }

    private bool IsValidLocation(Vector3 location, string towerName)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(location);

        if (!groundTilemap.HasTile(cellPosition))
        {
            Debug.Log("Not a ground tile");
            return false;
        }

        if (pathTilemap.HasTile(cellPosition))
        {
            Debug.Log("It's a path tile");
            return false;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(location, 0.35f, 1 << LayerMask.NameToLayer("Tower"));
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != currentTower)
            {
                Debug.Log("Overlaps another tower");
                return false;
            }
        }

        Collider2D hitPathCollider = Physics2D.OverlapCircle(location, 0.35f, 1 << LayerMask.NameToLayer("Pathing"));
        if (hitPathCollider != null)
        {
            Debug.Log("Overlaps the path");
            return false;
        }

        if (towerName.Contains("SpaceInvadersTower"))
        {
            BoxCollider2D towerCollider = currentTower.GetComponent<BoxCollider2D>();
            Vector2 size = towerCollider.size;
            Vector2 offset = towerCollider.offset;
            Vector2 trueCenter = (Vector2)location + offset;

            Collider2D hitCollider = Physics2D.OverlapBox(trueCenter, size, 0, LayerMask.GetMask("Pathing"));

            Debug.Log($"OverlapBox location: {trueCenter}, size: {size}");
            Debug.Log("OverlapBox hit: " + (hitCollider != null ? hitCollider.name : "None"));

            if (hitCollider != null)
            {
                Debug.Log("SpaceInvadersTower intersects with the path");
                return false;
            }
        }

        Debug.Log("Valid Location");
        return true;
    }

}
