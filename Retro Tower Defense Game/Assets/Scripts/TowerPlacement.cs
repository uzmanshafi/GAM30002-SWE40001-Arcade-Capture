using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    public GameObject[] towerPrefabs;
    public Tilemap groundTilemap;
    public Tilemap pathTilemap;
    public GameObject RadiusPrefab;
    private GameObject currentTower;
    private GameObject currentRadius;
    private Tower currentlySelectedTower = null;
    private UIManager uiManager;
    private SpriteRenderer currentTowerSpriteRenderer;
    private GameManager gameManager;
    private float scaleFactor = 0.5f;
    private float towerPlacedCooldown = 0.5f;

    //using these variables for star rating ui expection and animation
    private GameObject starRatingUI;
    private Vector3 originalStarRatingPos;
    private float moveDuration = 1.5f;
    private bool starRatingIsUp = false;
    private bool isMovingStarRating = false;

    [SerializeField] protected GameObject moneyText;
    [SerializeField] protected AudioClip place;
    [SerializeField] protected AudioMixerGroup soundGroup;

    //control indicators
    [SerializeField] private GameObject Controlindicator;

    //Pong Placement Indicators
    [SerializeField] private GameObject pongIndicatorPrefab;
    private LineRenderer pongIndicator;

    void Start()
    {
        gameManager = GameManager.instance;
        uiManager = UIManager.instance;

        starRatingUI = GameObject.FindGameObjectWithTag("starRating");
        Debug.Log("found" + " " + starRatingUI);

        if (starRatingUI != null)
        {
            originalStarRatingPos = starRatingUI.transform.position;
        }

    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        PlacementCooldown();
        CancelTowerPlacement();
        TowerDragging(mouseWorldPos);
        TowerRotation();
        RunTowerPlacement(mouseWorldPos);
        TowerSelection(mouseWorldPos);
    }

    private void PlacementCooldown()
    {
        if (towerPlacedCooldown > 0)
        {
            towerPlacedCooldown -= Time.deltaTime;
        }
    }

    private void CancelTowerPlacement()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
        {
            DestroyCurrentTower();
        }
    }

    private void TowerDragging(Vector3 mouseWorldPos)
    {
        if (currentTower != null && towerPlacedCooldown <= 0)
        {
            DragTower(mouseWorldPos);
        }
    }

    private void TowerRotation()
{
    if (currentTower != null)
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.mouseScrollDelta.y > 0)
        {
            // Anti-clockwise rotation for scroll up or 'R' key press
            currentTower.transform.Rotate(0, 0, -45);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            // Clockwise rotation for scroll down
            currentTower.transform.Rotate(0, 0, 45);
        }
    }
}


    private void RunTowerPlacement(Vector3 mouseWorldPos)
    {
        if (IsMouseOverUI()) return;

        if (currentTower != null && Input.GetMouseButtonUp(0) && IsValidLocation(mouseWorldPos, currentTower.name))
        {
            DropTower();
            towerPlacedCooldown = 0.5f; // Resets cooldown after placing a tower
        }
    }

    private void TowerSelection(Vector3 mouseWorldPos)
    {
        if (IsMouseOverUI()) return;

        if (Input.GetMouseButtonDown(0) && currentTower == null && towerPlacedCooldown <= 0)
        {
            AttemptSelectTower(mouseWorldPos);
        }
    }

    private void DestroyCurrentTower()
    {
        if (currentTower != null)
        {
            if (currentTower.TryGetComponent<PongTower>(out PongTower pt) && pt.other != null)
            {
                Destroy(pt.other.gameObject);
            }
            Destroy(currentTower);
            Destroy(currentRadius);
            currentTower = null;
            currentTowerSpriteRenderer = null;
        }

        Controlindicator.SetActive(false);
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
        if (gameManager.money - towerPrefab.GetComponent<Tower>().cost < 0)
        {
            return;
        }
        uiManager.deselectTower();
        DestroyCurrentRadius();

        currentTower = Instantiate(towerPrefab);
        currentTower.transform.position = position + new Vector3(1.5f, 1.5f, 0);
        currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
        Tower shootScript = currentTower.GetComponent<Tower>();
        shootScript.enabled = false;

        if (towerPrefab.name.StartsWith("SpaceInvaders"))
        {
            Transform lineRadius = currentTower.transform.Find("LineRadiusPrefab");
            if (lineRadius)
            {
                lineRadius.gameObject.SetActive(true); // Activating when spawned
                Debug.Log("line radius set true");
                SpaceInvaderTower spaceInvader = currentTower.GetComponent<SpaceInvaderTower>();
                if (spaceInvader != null)
                {

                    SetSpaceInvaderScale(lineRadius, spaceInvader);
                }
            }

            Transform PlacementIndicators = currentTower.transform.Find("PlacementIndicators");
            if (PlacementIndicators)
            {
                PlacementIndicators.gameObject.SetActive(true);
                Debug.Log("PlacementIndicators set true");
            }
        }
        else
        {
            SpawnIndicatorForTower(currentTower.GetComponent<Tower>());
            UpdateRadiusDisplay(currentTower.GetComponent<Tower>().range);
        }
        TextMeshProUGUI towerDescriptionTMP = Controlindicator.GetComponentInChildren<TextMeshProUGUI>(true);
        if (towerDescriptionTMP != null)
        {
            towerDescriptionTMP.text = currentTower.GetComponent<Tower>().towerDescription;
        }
    }

    private void DragTower(Vector3 newPosition)
    {
        currentTower.transform.position = newPosition;

        PongTower pt;
        if (!IsValidLocation(newPosition, currentTower.name)) //swapped to add extra check for pong tower, is now implicitly red else its green
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
        else if (currentTower.TryGetComponent<PongTower>(out pt) && Vector3.Distance(currentTower.transform.position, currentRadius.transform.position) > pt.range)
        {
            currentTowerSpriteRenderer.color = new Color(1, 0, 0, 0.8f);
        }
        else
        {
            currentTowerSpriteRenderer.color = new Color(0, 1, 0, 0.8f);

        }

        if (currentRadius)
        {
            if (currentTower.TryGetComponent<PongTower>(out pt) && pt.other != null)
            {

            }
            else
            {
                currentRadius.transform.position = newPosition;
            }
        }

        if (pongIndicator != null && currentTower.TryGetComponent<PongTower>(out pt) && pt.other != null)
        {
            pongIndicator.positionCount = 2;
            pongIndicator.SetPosition(0, Vector3.MoveTowards(pt.other.transform.position, pt.transform.position, .5f));
            pongIndicator.SetPosition(1, Vector3.MoveTowards(pt.transform.position, pt.other.transform.position, .5f));
        }

        // Checks the distance to the starRatingUI
        float distanceToStarRating = Vector3.Distance(currentTower.transform.position, starRatingUI.transform.position);
        float moveThreshold = 2f;

        if (distanceToStarRating < moveThreshold && !starRatingIsUp)
        {
            MoveStarRatingUp();
        }
        else if (distanceToStarRating >= moveThreshold && starRatingIsUp)
        {
            MoveStarRatingToOriginalPos();
        }

        Controlindicator.SetActive(true); // displays the control indicator
    }

    private void DropTower()
    {
        Debug.Log("DropTower called");

        // Deactivate the LineRadiusPrefab if the current tower is a "SpaceInvaders" tower
        if (currentTower && currentTower.name.StartsWith("SpaceInvaders"))
        {
            SoundEffect.PlaySoundEffect(place, currentTower.transform.position, 1, soundGroup);
            Transform childTransform = currentTower.transform.Find("LineRadiusPrefab");
            if (childTransform != null && childTransform.CompareTag("LineRadius"))
            {
                Debug.Log("Found LineRadiusPrefab. Deactivating it...");
                childTransform.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Couldn't find LineRadiusPrefab for deactivation.");
            }

            Transform PlacementIndicators = currentTower.transform.Find("PlacementIndicators");
            if (PlacementIndicators)
            {
                PlacementIndicators.gameObject.SetActive(false);
                Debug.Log("PlacementIndicators set false");
            }
        }

        MoveStarRatingToOriginalPos();
        Tower shootScript = currentTower.GetComponent<Tower>();

        if (currentTower.TryGetComponent<PongTower>(out PongTower pt) && pt.other == null)
        {
            currentTowerSpriteRenderer.color = Color.white;
            currentTower = Instantiate(towerPrefabs[5]);
            currentTower.transform.position = GetMouseWorldPosition();
            currentTowerSpriteRenderer = currentTower.GetComponent<SpriteRenderer>();
            pt.other = currentTower.GetComponent<PongTower>();
            pt.TowerOrder = 0;
            pt.other.other = pt;
            currentTower.GetComponent<Tower>().enabled = false;
            shootScript.enabled = true;
            pongIndicator = Instantiate(pongIndicatorPrefab, currentTower.transform).GetComponent<LineRenderer>();
        }
        else if (currentTower.TryGetComponent<PongTower>(out PongTower pt2) && pt2.other != null)
        {
            if (Vector2.Distance(pt2.transform.position, pt2.other.transform.position) <= pt2.other.range)
            {
                SoundEffect.PlaySoundEffect(place, currentTower.transform.position, 1, soundGroup);
                shootScript.enabled = true;
                if (!gameManager.AllTowers.Contains(shootScript))
                {
                    gameManager.AllTowers.Add(shootScript);
                    gameManager.money -= shootScript.cost;
                    GameObject moneyonKillText = Instantiate(moneyText, currentTower.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    Destroy(moneyonKillText, .9f);
                    moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().text = "-" + shootScript.cost;
                    moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                }
                pt2.TowerOrder = 1;
                currentTowerSpriteRenderer.color = Color.white;
                currentTower = null;
                currentTowerSpriteRenderer = null;
                Destroy(pongIndicator.gameObject);
                pongIndicator = null;
                DestroyCurrentRadius();
            }
        }
        else
        {
            SoundEffect.PlaySoundEffect(place, currentTower.transform.position, 1, soundGroup);
            if (!gameManager.AllTowers.Contains(shootScript))
            {
                gameManager.AllTowers.Add(shootScript);
                gameManager.money -= shootScript.cost;
                GameObject moneyonKillText = Instantiate(moneyText, currentTower.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                Destroy(moneyonKillText, .9f);
                moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().text = "-" + shootScript.cost;
                moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
            shootScript.enabled = true;
            currentTowerSpriteRenderer.color = Color.white;
            currentTower = null;
            currentTowerSpriteRenderer = null;
            DestroyCurrentRadius();
        }
        Controlindicator.SetActive(false);
    }

    private void AttemptSelectTower(Vector3 position)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Tower");
        Collider2D hitCollider = Physics2D.OverlapPoint(position, layerMask);

        if (hitCollider != null && hitCollider.gameObject.CompareTag("ArcadeTower"))
        {
            Tower towerScript = hitCollider.gameObject.GetComponent<Tower>();
            if (towerScript != null)
            {
                if (currentlySelectedTower == towerScript)
                {
                    uiManager.deselectTower();
                    DestroyCurrentRadius();
                    ToggleSpaceInvaderRadius(towerScript, false);  // Deactivates LineRadiusPrefab for the deselected tower
                    currentlySelectedTower = null;
                }
                else
                {
                    if (currentlySelectedTower)
                    {
                        uiManager.deselectTower();
                        DestroyCurrentRadius();
                        ToggleSpaceInvaderRadius(currentlySelectedTower, false);  // Deactivates LineRadiusPrefab for the previously selected tower
                    }
                    currentlySelectedTower = towerScript;
                    uiManager.selectTower(towerScript);
                    ToggleSpaceInvaderRadius(currentlySelectedTower, true);  // Activates LineRadiusPrefab for the newly selected tower

                    if (towerScript.gameObject.name.StartsWith("SpaceInvaders"))
                    {
                        Transform lineRadius = towerScript.transform.Find("LineRadiusPrefab");
                        if (lineRadius)
                        {
                            SpaceInvaderTower spaceInvader = towerScript.GetComponent<SpaceInvaderTower>();
                            if (spaceInvader != null)
                            {
                                SetSpaceInvaderScale(lineRadius, spaceInvader);
                            }
                        }
                    }
                    else
                    {
                        SpawnIndicatorForTower(towerScript);
                        UpdateRadiusDisplay(towerScript.range);
                    }
                }
            }
        }
        else if (currentlySelectedTower)
        {
            uiManager.deselectTower();
            DestroyCurrentRadius();
            ToggleSpaceInvaderRadius(currentlySelectedTower, false);  // Deactivates LineRadiusPrefab for the deselected tower
            currentlySelectedTower = null;
        }
    }

    private void SpawnIndicatorForTower(Tower tower)
    {
        DestroyCurrentRadius();

        if (tower.gameObject.name.StartsWith("SpaceInvaders"))
        {
            currentRadius = tower.transform.Find("LineRadiusPrefab").gameObject;
            if (currentRadius)
            {
                currentRadius.SetActive(true); // Ensure this line is executed
                SpaceInvaderTower spaceInvader = tower.GetComponent<SpaceInvaderTower>();
                if (spaceInvader != null)
                {
                    SetSpaceInvaderScale(currentRadius.transform, spaceInvader);
                }
            }
        }
        else
        {
            currentRadius = Instantiate(RadiusPrefab, tower.transform.position, Quaternion.identity);
            UpdateRadiusDisplay(tower.range);
            currentRadius.transform.SetParent(tower.transform);
        }
    }

    private void SetSpaceInvaderScale(Transform lineRadius, SpaceInvaderTower spaceInvader)
    {
        Vector2 calculatedScale = CalculateSpaceInvaderScale(spaceInvader);
        lineRadius.localScale = calculatedScale;
    }

    private Vector2 CalculateSpaceInvaderScale(SpaceInvaderTower spaceInvader)
    {
        float sightDistance = spaceInvader.SightDistance;
        // Adjust the values below as needed to match the visual representation you desire:
        float defaultXScale = 0.18f;
        float defaultYScale = 0.15f;
        float baseDistance = 1f;
        float newScaleY = defaultYScale * (sightDistance / baseDistance);
        return new Vector2(defaultXScale, newScaleY);
    }

    private bool IsMouseOverUI()
    {
        /*if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }*/ //removed as need to check whether its over the tower description specifically as well
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach(RaycastResult r in results)
        {
            if(r.gameObject.name == "towerDescription")
            {
                return false;
            }
        }
        return results.Count > 0;
    }


    private void UpdateRadiusDisplay(float range, bool isSpaceInvadersTower = false)
    {
        float desiredRadius = range * scaleFactor;

        if (isSpaceInvadersTower)
        {
            SpaceInvaderTower spaceInvader = currentTower.GetComponent<SpaceInvaderTower>();
            float sightDistance = (spaceInvader != null) ? spaceInvader.SightDistance : 0;
            currentRadius.transform.localScale = new Vector2(0.8f, sightDistance);
        }
        else
        {
            currentRadius.transform.localScale = new Vector2(desiredRadius, desiredRadius);
        }
    }
    public void DestroyCurrentRadius()
    {
        if (currentRadius)
        {
            if (!currentRadius.name.StartsWith("LineRadiusPrefab"))
            {
                Destroy(currentRadius);
            }
            else
            {
                currentRadius.SetActive(false);
            }
            currentRadius = null;
        }

        if (currentTower && currentTower.name.StartsWith("SpaceInvaders"))
        {
            Transform lineRadius = currentTower.transform.Find("LineRadiusPrefab");
            if (lineRadius)
            {
                lineRadius.gameObject.SetActive(false);
            }
        }
    }

    private void ToggleSpaceInvaderRadius(Tower tower, bool setActive)
    {
        if (tower.gameObject.name.StartsWith("SpaceInvaders"))
        {
            Transform lineRadius = tower.transform.Find("LineRadiusPrefab");
            if (lineRadius)
            {
                lineRadius.gameObject.SetActive(setActive);
            }
        }
    }

    public void DeactivateSpaceInvaderRadius(GameObject tower)
    {
        if (tower && tower.name.StartsWith("SpaceInvaders"))
        {
            Transform lineRadius = tower.transform.Find("LineRadiusPrefab");
            if (lineRadius)
            {
                lineRadius.gameObject.SetActive(false);
            }
        }
    }



    //star rating related function are here
    private void MoveStarRatingUp()
    {
        if (!starRatingIsUp && !isMovingStarRating) // Check if coroutine is not already running
        {
            StopAllCoroutines(); // Stops any existing move coroutines
            StartCoroutine(MoveStarRating(originalStarRatingPos, new Vector3(originalStarRatingPos.x, originalStarRatingPos.y + 1, originalStarRatingPos.z)));
            starRatingIsUp = true;
        }
    }

    private void MoveStarRatingToOriginalPos()
    {
        if (!isMovingStarRating) // Checks if coroutine is not already running
        {
            StopAllCoroutines(); // Stops any existing move coroutines
            StartCoroutine(MoveStarRating(starRatingUI.transform.position, originalStarRatingPos));
            starRatingIsUp = false;
        }
    }

    private IEnumerator MoveStarRating(Vector3 startPos, Vector3 endPos)
    {
        if (startPos == endPos) yield break;
        if (isMovingStarRating) yield break; // Checks if coroutine is already running
        isMovingStarRating = true;

        float journeyLength = Vector3.Distance(startPos, endPos);
        float startTime = Time.time;
        float distanceCovered;

        do
        {
            distanceCovered = (Time.time - startTime) * moveDuration;
            float fractionOfJourney = distanceCovered / journeyLength;
            starRatingUI.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);

            yield return null; // Waits for next frame
        }
        while (distanceCovered < journeyLength);

        starRatingUI.transform.position = endPos; // Ensures the final position is set correctly
        isMovingStarRating = false;
    }

    private bool IsValidLocation(Vector3 location, string towerName)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(location);

        if (!groundTilemap.HasTile(cellPosition))
        {
            return false;
        }

        if (pathTilemap.HasTile(cellPosition))
        {
            return false;
        }

        Collider2D[] overlaps = Physics2D.OverlapBoxAll(location, currentTowerSpriteRenderer.bounds.size, 0);
        foreach (var overlap in overlaps)
        {
            if (overlap.gameObject.CompareTag("Wall"))
            {
                return false;
            }


            if (overlap.gameObject.CompareTag("ArcadeTower") && overlap.gameObject != currentTower)
            {
                return false;
            }
        }

        if (towerName.Equals("SpaceInvadersTower"))
        {
            Collider2D[] towerOverlaps = Physics2D.OverlapBoxAll(location, currentTower.GetComponent<BoxCollider2D>().size, 0);
            foreach (var overlap in towerOverlaps)
            {
                if (overlap.gameObject.CompareTag("Wall") || overlap.gameObject.CompareTag("Path"))
                {
                    return false;
                }
            }
        }

        Collider2D towerCollider = currentTower.GetComponent<Collider2D>();

        Collider2D[] results = new Collider2D[10];

        int numResults = towerCollider.OverlapCollider(new ContactFilter2D(), results);

        for (int i = 0; i < numResults; i++)
        {
            if (results[i].gameObject.CompareTag("PathTilemap"))
            {
                return false;
            }
        }

        int uiLayerMask = 1 << LayerMask.NameToLayer("UI");
        Collider2D hitCollider = Physics2D.OverlapPoint(location, uiLayerMask);

        if (hitCollider != null)
        {
            if (hitCollider.transform.IsChildOf(starRatingUI.transform))
            {
                MoveStarRatingUp();
            }
            else
            {
                return false;
            }
        }

        return true;
    }

}