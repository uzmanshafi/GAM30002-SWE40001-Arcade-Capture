using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerPlaccement : MonoBehaviour
{
    public GameObject towerPrefab;
    private GameObject currentTower;
    private bool placingTower = false;

    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleTowerSpawn();
        }

        if (placingTower)
        {
            FollowWithMouse();
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
        }
    }

    private void ToggleTowerSpawn()
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

    private void FollowWithMouse()
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
            Vector3Int cellPos = tilemap.WorldToCell(currentTower.transform.position);

            if (!IsCellOccupied(cellPos))
            {
                Instantiate(towerPrefab, currentTower.transform.position, Quaternion.identity);
                Destroy(currentTower);
            }
        }
    }

    private bool IsCellOccupied(Vector3Int cellPos)
    {
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile != null)
        {
            if (tilemap.GetTileFlags(cellPos) == TileFlags.None)
            {
                return true;
            }
        }
        
        return false;
    }
}
