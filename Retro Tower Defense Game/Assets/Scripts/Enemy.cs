using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float MaxHP;
    
    [SerializeField] protected int moneyOnKill;

    protected GameManager GM;

    private Waypoints waypoints;
    private int i = 0; //index for heading position
    private Vector3 destination;
    protected float health;

    private bool is_camo;

    public float GetMovementSpeed => MovementSpeed; // Getter
    public Waypoints GetWaypoints => waypoints; // Getter (I would prefer the name to not be the same as the type)
    public Vector3 GetDestination => destination; //Getter 
    public int getWaypointIndex => i; //Getter
    public bool IsCamo => is_camo; //Getter

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    protected void init()
    {
        GM = GameManager.instance;
        waypoints = GM.GetComponent<Waypoints>();
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

    protected void move()
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

    internal virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GM.AllEnemies.Remove(this);
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

        GM.AllEnemies.Remove(this);
        GM.stars -= 0.5f;
        Destroy(gameObject);
    }
}
