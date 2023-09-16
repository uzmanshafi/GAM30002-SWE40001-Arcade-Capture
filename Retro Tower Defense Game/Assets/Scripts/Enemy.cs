using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float MaxHP;
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private int moneyOnKill;

    GameManager GM;

    private int i = 0; //index for heading position
    private Vector3 destination;
    private float health;

    public float GetMovementSpeed => MovementSpeed; // Getter
    public Waypoints GetWaypoints => waypoints; // Getter (I would prefer the name to not be the same as the type)
    public Vector3 GetDestination => destination; //Getter 
    public int getWaypointIndex => i; //Getter

    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        health = MaxHP;
        gameObject.SetActive(true);
        transform.position = waypoints.Points[i];
        i += 1;
        destination = waypoints.getWaypointPosition(i);
        rotate();
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    private void move()
    {
        //gameObject.GetComponent<Rigidbody2D>().N
        transform.position = Vector3.MoveTowards(transform.position, destination, MovementSpeed * Time.deltaTime);

        if (pointReached())
        {
            updateWaypoint();
        }
    }

    private bool pointReached()
    {
        float distanceToNext = (transform.position - destination).magnitude;
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
            destination = waypoints.getWaypointPosition(i);
            rotate();
        }
        else
        {
            endReached();
        }
    }

    internal void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GM.AllEnemies.Remove(gameObject);
            GM.money += moneyOnKill;
            //GameManager.instance.money += moneyOnKill;
            Destroy(gameObject);
            
        }
    }

    private void rotate()
    {
        transform.right = destination - transform.position;
    }

    private void endReached()
    {

        GM.AllEnemies.Remove(gameObject);
        GM.stars -= 0.5f;
        Destroy(gameObject);
    }
}
