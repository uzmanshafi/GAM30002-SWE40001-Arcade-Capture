using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanProjectiles : Projectile
{
    private Vector3 exitPoint;
    private Vector3 startPoint;

    private Vector3 targetPosition;

    public void SetExitPoint(Vector3 exitPoint)
    {
        this.exitPoint = exitPoint;
    }

    public void SetStartPoint(Vector3 startPoint)
    {
        this.startPoint = startPoint;
    }

    void Start()
    {
        targetPosition = startPoint; // Initially move towards the start point
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Toggle target position when reached
            if (targetPosition == startPoint)
            {
                targetPosition = exitPoint;
            }
            else
            {
                targetPosition = startPoint;
            }
        }
        move();
    }

    public override void move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
    
    // Implement OnTriggerEnter2D to handle enemy collisions, etc...
}