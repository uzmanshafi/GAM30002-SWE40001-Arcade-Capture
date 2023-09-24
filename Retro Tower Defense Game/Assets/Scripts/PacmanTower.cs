using System.Collections;
using UnityEngine;

public class PacmanTower : Tower
{
    private GameManager gameManager;
    private Waypoints waypoints;
    private Vector3 exitPoint;

    private void Start()
    {
        // Retrieve the GameManager and Waypoints
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        waypoints = gameManager.GetComponent<Waypoints>();

        exitPoint = waypoints.Points[waypoints.Points.Length - 1]; // Assuming the last point is the exit
        
        // Set default cooldown to 6 seconds
        cooldown = 6;
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown)
        {
            SpawnGhost(); // Spawn the first ghost
            SpawnGhost(); // Spawn the second ghost

            lastShotTime = Time.time;
        }
    }

    private void SpawnGhost()
    {
        // Randomly select one of the bullet types (which are actually ghosts)
        GameObject ghostPrefab = bulletTypes[Random.Range(0, bulletTypes.Length)];
        GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);

        GhostProjectiles ghostScript = ghost.GetComponent<GhostProjectiles>();
        ghostScript.SetExitPoint(exitPoint);
        ghostScript.SetWaypoints(waypoints.Points);
    }
}
