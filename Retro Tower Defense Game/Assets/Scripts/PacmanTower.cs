using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanTower : Tower
{
    [Header("Base Configuration")]
    [SerializeField] private float baseGhostSpeed = 1.0f;
    [SerializeField] private float baseGhostDamage = 1.0f;

    private GameManager gameManager;
    private Waypoints waypoints;
    private Vector3 exitPoint;

    private Wave waveScript;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        waveScript = gameManager.GetComponent<Wave>();
        waypoints = gameManager.GetComponent<Waypoints>();
        exitPoint = waypoints.Points[waypoints.Points.Length - 1];
        base.init();
    }

    private void Update()
    {
        ConfigureBasedOnUpgrade();
        tryShoot();
    }

    protected override void tryShoot()
    {
        // Checks if the wave is not in progress. If it isn't, doesn't spawn ghosts.
        if (!waveScript.waveInProgress)
        {
            return;
        }

        if (Time.time - lastShotTime > cooldown)
        {
            switch (upgradeLevel)
            {
                case 0:
                    StartCoroutine(ShootGhostsLevel1());
                    break;
                case 1:
                    StartCoroutine(ShootGhostsLevel2());
                    break;
                case 2:
                    SpawnGhostForCurrentLevel();  // Spawns all at once but faster and with more damage
                    break;
            }
            lastShotTime = Time.time;
        }
    }

    private IEnumerator ShootGhostsLevel1()
    {
        SpawnGhost(baseGhostSpeed, baseGhostDamage, bulletTypes[0]); // Red Ghost
        yield return new WaitForSeconds(1);
        SpawnGhost(baseGhostSpeed, baseGhostDamage, bulletTypes[1]); // Blue Ghost
    }

    private IEnumerator ShootGhostsLevel2()
    {
        for (int i = 0; i < 4; i++)
        {
            SpawnGhost(baseGhostSpeed * 1.25f, baseGhostDamage, bulletTypes[i]);
            yield return new WaitForSeconds(1);
        }
    }

    void SpawnGhostForCurrentLevel()
    {
        float speedMultiplier = upgradeLevel == 2 ? 1.75f : 1f;
        float damageMultiplier = upgradeLevel == 2 ? 1.75f : 1f;

        foreach (var ghostPrefab in bulletTypes)
        {
            SpawnGhost(baseGhostSpeed * speedMultiplier, baseGhostDamage * damageMultiplier, ghostPrefab);
        }
    }

    void ConfigureBasedOnUpgrade()
    {
        cooldown = 6.0f;
    }

    void SpawnGhost(float speed, float damage, GameObject ghostPrefab)
    {
        Vector3 spawnPoint = GetLastWaypointInRange();
        GameObject ghost = Instantiate(ghostPrefab, spawnPoint, Quaternion.identity);

        GhostProjectiles ghostScript = ghost.GetComponent<GhostProjectiles>();
        ghostScript.SetExitPoint(spawnPoint);  // Set the spawn point as the exit point
        ghostScript.SetWaypoints(waypoints.Points);
        ghostScript.SetStartWaypoint(waypoints.Points.Length - 1);
        ghostScript.SetStats(speed, damage);
        ghost.SetActive(false);
        gameManager.AddGhost(ghost);
    }

    private Vector3 GetLastWaypointInRange()
    {
        Vector3? lastWaypointInRange = null;
        foreach (Vector3 waypoint in waypoints.Points)
        {
            if (Vector2.Distance(transform.position, waypoint) <= range)
            {
                lastWaypointInRange = waypoint;
                Debug.Log($"Waypoint {waypoint} is within range.");
            }
            else
            {
                Debug.Log($"Waypoint {waypoint} is out of range. Breaking...");
                break;
            }
        }

        if (lastWaypointInRange.HasValue)
        {
            Debug.Log($"Last waypoint in range: {lastWaypointInRange.Value}");
            return lastWaypointInRange.Value;
        }
        else
        {
            Debug.Log("No waypoint in range. Defaulting to exit point.");
            return waypoints.Points[waypoints.Points.Length - 1];
        }
    }


}
