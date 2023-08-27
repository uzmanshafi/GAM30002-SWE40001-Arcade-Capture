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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleTowerPlacementMode();
        }

        if (placingTower)
        {
            HandleTowerPlacement();
            HandleTowerRotation();
        }
        else if (movingTower)
        {
            HandleTowerMove();
            HandleTowerRotation();
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
        Vector3Int cellPosition = groundTilemap.WorldToCell(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsCellValid(cellPosition))
            {
                currentTower.transform.position = groundTilemap.GetCellCenterWorld(cellPosition);
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
        Vector3Int cellPosition = groundTilemap.WorldToCell(mousePos);

        if (Input.GetMouseButtonDown(0))
        {
            if (IsCellValid(cellPosition))
            {
                towerToMove.transform.position = groundTilemap.GetCellCenterWorld(cellPosition);
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

    private void HandleTowerRotation()
    {
        if (Input.GetKeyDown(KeyCode.R) && (placingTower || movingTower))
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
    }

    private bool IsCellValid(Vector3Int cellPosition)
    {
        TileBase groundTile = groundTilemap.GetTile(cellPosition);
        TileBase topTile = topTilemap.GetTile(cellPosition);
        
        return groundTile != null && topTile == null;
    }
}
