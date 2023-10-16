using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using static Utils;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float MaxHP;
    [SerializeField] protected AudioClip die;
    [SerializeField] protected AudioMixerGroup soundGroup;

    [SerializeField] protected GameObject coinplosion;
    [SerializeField] protected float coinParticleMultiplier = 1f;

    [SerializeField] protected int moneyOnKill;
    [SerializeField] protected GameObject moneyText;

    protected GameManager GM;

    private Waypoints waypoints;
    private int i = 0; //index for heading position
    private Vector3 destination;
    protected float health;
    protected bool isDead = false;

    private bool is_camo;

    [NonSerialized] public bool spawnAtStart = true;

    public float GetMovementSpeed => MovementSpeed; // Getter
    public Waypoints GetWaypoints => waypoints; // Getter (I would prefer the name to not be the same as the type)
    public Waypoints SetWaypoints
    {
        set { waypoints = value; }
    }

    public Vector3 GetDestination => destination; //Getter 
    public Vector3 SetDestination
    {
        set { destination = value; }
    }

    public int getWaypointIndex => i; //Getter
    public int setWaypointIndex
    {
        set { i = value; }
    }

    public bool IsCamo => is_camo; //Getter

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
        
        if (spawnAtStart)
        {
            transform.position = waypoints.Points[i];
            updateWaypoint();
        }
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

        if (pointReached(transform.position, destination, 0.001f))
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, MovementSpeed * Time.deltaTime);
            updateWaypoint();
        }
    }

    private void updateWaypoint()
    {
        int finalWaypointIndex = waypoints.Points.Length - 1;
        if (i < finalWaypointIndex)
        {

            i++;
            if (waypoints.getWaypointPosition(i - 1).x == waypoints.getWaypointPosition(i).x) //if current position and next position are vertial, aim slightly left to avoid veering off path
            {
                destination = new Vector3(waypoints.getWaypointPosition(i).x + .1f, waypoints.getWaypointPosition(i).y);
            }
            else if(waypoints.getWaypointPosition(i).x == waypoints.getWaypointPosition(i + 1).x) // if destination and waypoint after that are equal in x, head a little further to avoid turning early
            {
                destination = new Vector3(waypoints.getWaypointPosition(i).x + .1f, waypoints.getWaypointPosition(i).y);
            }
            else
            {
                destination = waypoints.getWaypointPosition(i);
            }
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
        if (health <= 0 && !isDead)
        {
            isDead = true;
            GM.AllEnemies(true).Remove(this);
            GM.money += moneyOnKill;
            GameObject moneyonKillText = Instantiate(moneyText, transform.position, Quaternion.identity);
            Destroy(moneyonKillText, .9f);
            moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + moneyOnKill;
            SoundEffect.PlaySoundEffect(die, transform.position, 1, soundGroup);
            GameObject effect = Instantiate(coinplosion, transform.position + new Vector3(0,0,-0.1f), Quaternion.identity);
            effect.GetComponent<ParticleSystem>().Emit((int)(moneyOnKill * coinParticleMultiplier));
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

        GM.AllEnemies(true).Remove(this);
        GM.stars -= 1f;
        Destroy(gameObject);
    }
}
