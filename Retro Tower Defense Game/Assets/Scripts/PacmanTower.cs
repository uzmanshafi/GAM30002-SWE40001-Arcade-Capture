using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanTower : Tower
{
    [SerializeField] private GameObject pacmanProjectilePrefab; // Reference to the Pac-Man Projectile Prefab
    [SerializeField] private Waypoints waypoints; // Reference to the Waypoints script
    private Vector3 exitPoint; // The exit point where projectiles are spawned
    private Vector3 startPoint; // The start point for returning

    [SerializeField] private int numGhosts = 2; // Number of ghosts at level 1

    void Start()
    {
        init(); // Call parent class initialization if required

        // Initialize start and exit points from waypoints
        exitPoint = waypoints.Points[waypoints.Points.Length - 1]; // Last point is the exit
        startPoint = waypoints.Points[0]; // First point is the start
    }

    void Update()
    {
        Enemy possibleEnemy = furthestTarget();
        if (possibleEnemy != null)
        {
            tryShoot();
        }
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown)
        {
            for (int i = 0; i < numGhosts; i++)
            {
                // Instantiate a new projectile at the exit point and pass the start and exit points
                GameObject projectileObj = Instantiate(pacmanProjectilePrefab, exitPoint, Quaternion.identity);
                PacmanProjectiles projectile = projectileObj.GetComponent<PacmanProjectiles>();
                projectile.SetExitPoint(exitPoint);
                projectile.SetStartPoint(startPoint);
            }

            lastShotTime = Time.time;
        }
    }
}
