using System;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] protected float damage = 1;
    [SerializeField] protected float cost;
    [SerializeField] protected float cooldown = 1;

    protected float lastShotTime; //used to determine cooldown
    [SerializeField] private Enemy target;

    [SerializeField] public float towerRadius;
    [SerializeField] public float range = 5;

    [SerializeField] protected GameObject[] bulletTypes;

    // Start is called before the first frame update
    void Start()
    {

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
            GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity );
            Projectile projectile = bullet.GetComponent<Projectile>(); 
            projectile.target = target;
            lastShotTime = Time.time;
        }
        
    }

    private void furthestTarget()
    {

        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, range, Vector2.up, LayerMask.GetMask("Enemy")); //Raycast and return any objects on layer enemy: Check if raycast is efficient
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
                endPoint = e.GetWaypoints.Points[e.GetWaypoints.Points.Length - 1];
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
