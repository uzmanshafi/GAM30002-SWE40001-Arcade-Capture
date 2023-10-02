using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProjectiles : Projectile
{
    [SerializeField] private Sprite[] sprites;

    private int currentWaypointIndex;
    private Vector3[] waypoints;
    private Vector3 exitPoint;
    private bool hasReachedExit;

    private float ghostSpeed = 1.0f;
    private float ghostDamage = 1.0f;

    private Rigidbody2D rb;
    [SerializeField] private AudioClip die;

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
            SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
            if (direction.x > 0.4 && direction.y > 0.4) // Second 
            {
                render.sprite = sprites[1];
            } 
            else if (direction.x < -0.4 && direction.y > 0.4) // Fourth 
            {
                render.sprite = sprites[3];
            }
            else if (direction.x < -0.4 && direction.y < -0.4) // Sixth 
            {
                render.sprite = sprites[5];
            }
            else if (direction.x > 0.4 && direction.y < -0.4) // Eighth 
            {
                render.sprite = sprites[7];
            }
            else if (direction.x > 0.9) // First 
            {
                render.sprite = sprites[0];
            }
            else if (direction.y > 0.9) // Third 
            {
                render.sprite = sprites[2];
            }
            else if (direction.x < -0.9) // Fifth 
            {
                render.sprite = sprites[4];
            }
            else if (direction.y < -0.9) // Seventh 
            {
                render.sprite = sprites[6];
            }
        }
        else
        {
            //Teleport to exit point*
            transform.position = exitPoint;
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

    public void SetStats(float speed, float damage)
    {
        ghostSpeed = speed;
        ghostDamage = damage;
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
                AudioSource.PlayClipAtPoint(die, transform.position);
                Destroy(gameObject);  // Destroy the ghost upon collision
            }
        }
    }
}
