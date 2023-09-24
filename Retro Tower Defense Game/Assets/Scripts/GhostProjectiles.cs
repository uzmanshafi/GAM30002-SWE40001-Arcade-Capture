using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProjectiles : Projectile
{
    private int currentWaypointIndex;
    private Vector3[] waypoints;
    private Vector3 exitPoint;
    private bool hasReachedExit;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetExitPoint(Vector3 point)
    {
        exitPoint = point;
    }

    public void SetWaypoints(Vector3[] points)
    {
        waypoints = points;
    }

    private void Start()
    {
        // Initially set direction to the exit point
        direction = ((Vector2)exitPoint - (Vector2)transform.position).normalized;
    }


    void Update()
    {
        if (hasReachedExit)
        {
            // Move along the waypoints from exit to start
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex]) < 0.1f)
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    // Reached the start, destroy the ghost
                    Destroy(gameObject);
                    return;
                }
            }

            // Update direction to the next waypoint
            direction = ((Vector2)(waypoints[currentWaypointIndex] - transform.position)).normalized;
        }
        else
        {
            // Move towards the exit point
            if (Vector3.Distance(transform.position, exitPoint) < 0.1f)
            {
                // Reached the exit point, set flag and initialize waypoint index
                hasReachedExit = true;
                currentWaypointIndex = waypoints.Length - 2;  // Skip the last waypoint (exit point)
            }
        }

        move();
    }


    public override void move()
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo)
            {
                e.TakeDamage(damage);
                Destroy(gameObject);  // Destroy the ghost upon collision
            }
        }
    }
}
