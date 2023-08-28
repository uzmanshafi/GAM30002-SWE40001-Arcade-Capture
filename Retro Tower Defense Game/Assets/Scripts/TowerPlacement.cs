using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    private GameObject currentTower;
    private bool placingTower = false;
    private bool movingTower = false;
    private GameObject towerToMove;

    public Tilemap groundTilemap;
    public Tilemap topTilemap;

    public LayerMask topTilemapLayerMask;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleTowerPlacementMode();
        }

        if (Input.GetKeyDown(KeyCode.R) && (placingTower || movingTower))
        {
            RotateTowerBeforePlacement();
        }

        if (placingTower)
        {
            HandleTowerPlacement();
        }
        else if (movingTower)
        {
            HandleTowerMove();
        }
        else
        {
            HandleTowerSelection();
        }
    }

    private void ToggleTowerPlacementMode()
    {
        if (!placingTower && !movingTower)
        {
            placingTower = true;
            currentTower = Instantiate(towerPrefab);
            currentTower.SetActive(true);
        }
        else
        {
            Destroy(currentTower);
            placingTower = false;
            movingTower = false;
            towerToMove = null;
        }
    }

    private void HandleTowerPlacement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Convert mouse position to cell coordinates
        Vector3Int cellPosition = groundTilemap.WorldToCell(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsCellValid(cellPosition))
            {
                currentTower.transform.position = mousePos;
                placingTower = false;
                Debug.Log("Tower placed successfully.");
            }
            else
            {
                Debug.Log("Cannot place tower in this location.");
            }
        }
        else
        {
            currentTower.transform.position = mousePos;
        }
    }

    private void HandleTowerMove()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Converts mouse position to cell coordinates
        Vector3Int cellPosition = groundTilemap.WorldToCell(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsCellValid(cellPosition))
            {
                towerToMove.transform.position = mousePos;
                movingTower = false;
                towerToMove = null;
            }
        }
        else
        {
            towerToMove.transform.position = mousePos;
        }
    }

    private void HandleTowerSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Tower"))
                {
                    towerToMove = hit.collider.gameObject;
                    movingTower = true;
                    break;
                }
            }
        }
    }

    private void RotateTowerBeforePlacement()
    {
        if (currentTower != null)
        {
            currentTower.transform.Rotate(Vector3.forward, 45f);
        }
        else if (towerToMove != null)
        {
            towerToMove.transform.Rotate(Vector3.forward, 45f);
        }
    }
    private bool IsCellValid(Vector3Int cellPosition)
    {
        TileBase groundTile = groundTilemap.GetTile(cellPosition);
        TileBase topTile = topTilemap.GetTile(cellPosition);

        if (groundTile != null && topTile == null)
        {
            Vector3 cellCenter = topTilemap.GetCellCenterWorld(cellPosition);
            Vector3 towerPosition = currentTower.transform.position;

            // Define a ray direction from the tower position to the cell center
            Vector3 rayDirection = (cellCenter - towerPosition).normalized;

            // Cast a ray from the tower position to the cell center
            RaycastHit2D hit = Physics2D.Raycast(towerPosition, rayDirection, Vector3.Distance(cellCenter, towerPosition), topTilemapLayerMask);

            if (hit.collider == null)
            {
                // No obstacles in the way, the placement is valid
                return true;
            }
        }

        return false;
    }
}
