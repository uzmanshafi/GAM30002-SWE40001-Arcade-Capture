using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] protected float damage = 1;
    [SerializeField] protected float cost;
    [SerializeField] protected float cooldown = 1;

    private float lastShotTime; //used to determine cooldown
    [SerializeField] private Enemy target;

    [SerializeField] public float towerRadius;
    [SerializeField] public float range = 5;

    public Vector2 position; //I think position variable will always be useless considering we have access to transform.position
    
    

    [SerializeField] private GameObject[] bulletTypes;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        fire();
    }

    private void fire() //abstract?
    {
        furthestTarget();
        if (Time.time - lastShotTime > cooldown && target != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletTypes[0], transform.position + dir, Quaternion.identity );
            HomingProjectile projectile = bullet.GetComponent<HomingProjectile>(); //Shoots homing projectile since I havent made 'Basic Projectile' and can't use abstract classes as components.
            projectile.target = target;
            lastShotTime = Time.time;
        }
        
    }

    private void furthestTarget()
    {

        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, 100, Vector2.up, LayerMask.GetMask("Enemy")); //Raycast and return any objects on layer enemy
        if (results.Length == 0)
        {
            target = null;
        }
        Enemy[] enemies_in_range = new Enemy[results.Length];

        for (int i = 0; i < results.Length; i++) //Create and populate array of enemies
        {
            Enemy e;
            if (results[i].collider.gameObject.TryGetComponent<Enemy>(out e))
            {
                enemies_in_range[i] = e;
            }
            //r.collider.gameObject.GetComponent<Enemy>().waypoints.Points[r.collider.gameObject.GetComponent<Enemy>().waypoints.Points.Length];
        }

        float bestDistance = Mathf.Infinity;
        float tempDistance;
        Vector3 endPoint;
        if (enemies_in_range.Length > 0)
        {
            foreach (Enemy e in enemies_in_range) // find enemy closest to end point (Distance not furthest on path)
            {
                if (e == null) { continue; }
                endPoint = e.waypoints.Points[e.waypoints.Points.Length - 1];
                tempDistance = Vector2.Distance(e.transform.position, endPoint);
                if (tempDistance < bestDistance)
                {
                    bestDistance = tempDistance;
                    target = e;
                }
            }
        }
    }

}
