using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private Waypoints waypoints;

    private int i = 0;
    private Vector3 currentPointHeading;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
        transform.position = waypoints.Points[i];
        i += 1;
        currentPointHeading = waypoints.getWaypointPosition(i);
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    private void move()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPointHeading, MovementSpeed * Time.deltaTime);

        if (pointReached())
        {
            updateWaypoint();
        }
    }

    private bool pointReached()
    {
        float distanceToNext = (transform.position - currentPointHeading).magnitude;
        if (distanceToNext < 0.1f)
        {
            return true;
        }
        return false;
    }

    private void updateWaypoint()
    {
        int finalWaypointIndex = waypoints.Points.Length - 1;
        if (i < finalWaypointIndex)
        {
            i++;
            currentPointHeading = waypoints.getWaypointPosition(i);
            rotate();
        }
        else
        {
            endReached();
        }
    }

    private void rotate()
    {
        if(currentPointHeading.x > waypoints.getWaypointPosition(i - 1).x)
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
    }

    private void endReached()
    {
        gameObject.SetActive(false);
    }
}
