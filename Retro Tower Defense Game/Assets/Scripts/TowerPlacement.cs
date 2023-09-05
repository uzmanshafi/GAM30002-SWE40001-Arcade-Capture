using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject arcadeTowerPrefab;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    private GameObject currentTower;

    private SpriteRenderer currentTowerSpriteRenderer;


    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnTower(mouseWorldPos);
        }

        if (currentTower != null)
        {
            DragTower(mouseWorldPos);

            if (Input.GetMouseButtonDown(0))
            {
                if (IsValidLocation(mouseWorldPos))
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

    public void SpawnTower(Vector3 position)
    {
        if (currentTower == null)
        {
            currentTower = Instantiate(arcadeTowerPrefab, position, Quaternion.identity);
            currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
        }
    }


    private void DragTower(Vector3 newPosition)
    {
        currentTower.transform.position = newPosition;
        if (IsValidLocation(newPosition))
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
        Collider2D hitCollider = Physics2D.OverlapPoint(position);
        if (hitCollider != null && hitCollider.gameObject.CompareTag("ArcadeTower"))
        {
            Destroy(hitCollider.gameObject);
            SpawnTower(position);
        }
    }

    private void RotateTower()
    {
        if (currentTower != null)
        {
            currentTower.transform.Rotate(0, 0, 45);
        }
    }

    private bool IsValidLocation(Vector3 location)
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

        // Checks for overlap with other towers
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

        Debug.Log("Valid Location");
        return true;
    }



}
