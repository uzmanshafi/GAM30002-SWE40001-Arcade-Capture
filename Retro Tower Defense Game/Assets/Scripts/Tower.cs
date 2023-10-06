using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TowerUpgrade
{
    public int cost;
    public string description;
}

public abstract class Tower : MonoBehaviour
{
    [SerializeField] public int cost;
    [SerializeField] public float cooldown = 1;
    [SerializeField] public float damage = 1;
    [SerializeField] public float bulletLifetime;

    [NonSerialized] public float base_cooldown;
    [NonSerialized] public float base_damage;

    protected float lastShotTime; //used to determine cooldown
    [SerializeField] protected Enemy target;

    [SerializeField] public float towerRadius;
    [SerializeField] public float range = 5;

    [NonSerialized] public float base_range;
    [SerializeField] public float radius;
    [SerializeField] public GameObject radiusDisplay;
    [SerializeField] protected GameObject[] bulletTypes;

    protected bool canSeeCamo;

    // public int upgrade
    [SerializeField] public string controlColour;


    [SerializeField] public Sprite inspectSprite;
    [SerializeField] public int upgradeLevel = 0;
    [SerializeField] public TowerUpgrade[] upgrades;

    // For description to display when placing
    [SerializeField] public string towerDescription;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    protected void init()
    {
        base_cooldown = cooldown;
        base_range = range;
        base_damage = damage;

    }

    // Update is called once per frame
    void Update()
    {
        tryShoot();
    }

    public void selected(bool t)
    {
        radiusDisplay.SetActive(t);
        radiusDisplay.transform.localScale = new Vector2(range * 2, range * 2);
    }


    protected abstract void tryShoot();

    protected Enemy? furthestTarget()
    {

        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, range, Vector2.up, LayerMask.GetMask("Enemy")); //Raycast and return any objects on layer enemy: Check if raycast is efficient
        if (results.Length == 0)
        {
            target = null;
        }
        List<Enemy> enemies_in_range = new List<Enemy>();

        for (int i = 0; i < results.Length; i++) //Create and populate array of enemies
        {
            Enemy e;
            Scroller s;
            if (results[i].collider.gameObject.TryGetComponent<Enemy>(out e))
            {
                /*Debug.Log(Vector2.Distance(e.transform.position, transform.position) < range);  This was returning false most of the time
                if (e.TryGetComponent<Scroller>(out s))
                {
                    if (s.colour == controlColour)
                    {
                        enemies_in_range.Add(s);
                    }
                }
                else
                {
                    enemies_in_range.Add(e);
                }*/
                if (Vector2.Distance(e.transform.position, transform.position) < range)
                {
                    enemies_in_range.Add(e);
                }

            }
            //r.collider.gameObject.GetComponent<Enemy>().waypoints.Points[r.collider.gameObject.GetComponent<Enemy>().waypoints.Points.Length];
        }

        float bestDistance = Mathf.Infinity;
        Enemy bestEnemy = null;
        float tempDistance;
        Vector3 endPoint;
        if (enemies_in_range.Count > 0)
        {
            foreach (Enemy e in enemies_in_range) // find enemy closest to end point (Distance not furthest on path)
            {
                if (e == null) { continue; }
                if (e.IsCamo && !canSeeCamo) { continue; }
                endPoint = e.GetWaypoints.Points[e.GetWaypoints.Points.Length - 1];
                tempDistance = Vector2.Distance(e.transform.position, endPoint);
                if (tempDistance < bestDistance)
                {
                    bestDistance = tempDistance;
                    bestEnemy = e;
                }
            }
            return bestEnemy;
        }
        return null;
    }
}
