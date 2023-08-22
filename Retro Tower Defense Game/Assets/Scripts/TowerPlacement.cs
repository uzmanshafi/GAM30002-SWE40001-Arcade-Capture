using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    private GameObject currentTower;
    private bool placingTower = false;

    public Tilemap groundTilemap; 
    public Tilemap topTilemap;

    private void Start()
    {
        groundTilemap = GetComponentInChildren<Tilemap>();
        topTilemap = GetComponentInChildren<Tilemap>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleTowerPlacementMode();
        }

        if (placingTower)
        {
            MoveTowerWithCursor();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
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

    private void PlaceTower()
    {
        if (currentTower != null)
        {
            Vector3Int cellPosition = groundTilemap.WorldToCell(currentTower.transform.position);

            if (!IsCellOccupied(cellPosition))
            {
                Instantiate(towerPrefab, groundTilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
                Destroy(currentTower);
            }
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
