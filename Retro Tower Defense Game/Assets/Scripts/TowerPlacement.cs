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
        }
        else if (movingTower)
        {
            HandleTowerMove();
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
                currentTower.transform.position = groundTilemap.GetCellCenterWorld(cellPosition);
                movingTower = false;
            }
        }
        else
        {
            currentTower.transform.position = mousePos;
        }
    }

    private bool IsCellValid(Vector3Int cellPosition)
    {
        TileBase groundTile = groundTilemap.GetTile(cellPosition);
        TileBase topTile = topTilemap.GetTile(cellPosition);
        
        
        return groundTile != null && topTile == null;
    }
}
