using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float MaxHP;
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moneyOnKill;

    GameManager GameManager;

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
            //GameObject.FindAnyObjectByType<samplePooler>().removeMe(gameObject);
            GameManager GM = GameObject.FindAnyObjectByType<GameManager>();
            GM.AllEnemies.Remove(gameObject);
            GM.money += 1;
            Destroy(gameObject);
            GameManager.instance.money += moneyOnKill;
        }
    }

    private void rotate()
    {
        transform.right = destination - transform.position;
        /*
        if(destination.x > waypoints.getWaypointPosition(i - 1).x)
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        */
    }

    private void endReached()
    {

        //GameObject.FindAnyObjectByType<Health>().TakeDamage((int)MaxHP);

        //GameObject.FindAnyObjectByType<samplePooler>().removeMe(gameObject);
        GameManager GM = GameObject.FindAnyObjectByType<GameManager>();
        GM.AllEnemies.Remove(gameObject);
        GM.health -= 1;
        Destroy(gameObject);
    }
}
