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
            // Checks if the cell is within the bounds of the top tilemap collider
            Vector3 cellCenter = topTilemap.GetCellCenterWorld(cellPosition);
            Collider2D topTileCollider = Physics2D.OverlapPoint(cellCenter, topTilemapLayerMask);

            // Checks if the cell is within the bounds of the arcade prefab's collider
            Collider2D arcadeCollider = Physics2D.OverlapPoint(cellCenter);

            if (topTileCollider == null && arcadeCollider == null)
            {
                return true;
            }
        }

        return false;
    }
}
