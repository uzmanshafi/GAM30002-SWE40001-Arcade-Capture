using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    private GameObject currentTower;
    private bool placingTower = false;
    private bool canPlaceTower = true;

    public Tilemap groundTilemap;
    public Tilemap topTilemap;

    private GameObject towerToMove;

    private LayerMask towerLayerMask;

    private void Start()
    {
        towerLayerMask = LayerMask.GetMask("Tower"); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleTowerPlacementMode();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (towerToMove != null)
            {
                HandleTowerMove();
            }
            else if (placingTower)
            {
                HandleTowerPlacement();
            }
            else
            {
                HandleTowerSelection();
            }
        }

        if (placingTower)
        {
            MoveTowerWithCursor();
        }
    }

    private void ToggleTowerPlacementMode()
    {
        placingTower = !placingTower;

        if (placingTower)
        {
            if (currentTower == null)
            {
                currentTower = Instantiate(towerPrefab);
                currentTower.SetActive(false);
            }
        }
        else
        {
            Destroy(currentTower);
        }

        towerToMove = null;
    }

    private void HandleTowerPlacement()
    {
        if (canPlaceTower) // Check if tower can be placed
        {
            if (currentTower != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = groundTilemap.WorldToCell(mousePos);

                if (!IsCellOccupied(cellPosition))
                {
                    Vector3 towerPosition = groundTilemap.GetCellCenterWorld(cellPosition);
                    towerPosition.y += groundTilemap.cellSize.y / 2;
                    Instantiate(towerPrefab, towerPosition, Quaternion.identity);
                    Destroy(currentTower);

                    canPlaceTower = false; // Disable placing until the tower is moved or another tower is placed
                }
            }
        }
    }

    private void HandleTowerSelection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity, towerLayerMask);

        Debug.Log("Number of colliders hit: " + hits.Length);

        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("Hit: " + hit.collider);

            if (hit.collider.CompareTag("Tower"))
            {
                Debug.Log("Tower clicked!");
            }
        }

        if (hits.Length > 0 && hits[0].collider.CompareTag("Tower"))
        {
            towerToMove = hits[0].collider.gameObject;
        }
    }

    private void HandleTowerMove()
    {
        if (towerToMove != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            towerToMove.transform.position = mousePos;

            if (Input.GetMouseButtonUp(0))
            {
                towerToMove = null; // Release the tower
                canPlaceTower = true; // Enable placing again once the tower is moved
            }
        }
    }

    private void MoveTowerWithCursor()
    {
        if (currentTower != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            currentTower.transform.position = mousePos;
            currentTower.SetActive(true);
        }
    }

    private bool IsCellOccupied(Vector3Int cellPosition)
    {
        TileBase groundTile = groundTilemap.GetTile(cellPosition);
        TileBase topTile = topTilemap.GetTile(cellPosition);

        if (groundTile != null && topTile == null)
        {
            if (groundTilemap.GetTileFlags(cellPosition) == TileFlags.None)
            {
                return true;
            }
        }

        return false;
    }
}
