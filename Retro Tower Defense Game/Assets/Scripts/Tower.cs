using System;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] protected float cost;
    [SerializeField] protected float cooldown = 1;

    protected float lastShotTime; //used to determine cooldown
    [SerializeField] protected Enemy target;

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
        tryShoot();
    }

    protected abstract void tryShoot();

    protected Enemy? furthestTarget()
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
                    return e;
                }
            }
        }
        return null;
    }

    private (Vector2? collision_point, float? time) GetCollisionPoint(Vector2 enemy_position, Vector2 enemy_direction, float enemy_speed, float projectile_speed,float elapsed_time)
    {
        //The vector from Enemy to Tower
        Vector2 ET = (Vector2)transform.position - enemy_position;

        float dot_product = Vector2.Dot(enemy_direction, ET);

        float speed_squared_difference = (float)(Math.Pow(enemy_speed, 2) - Math.Pow(projectile_speed, 2));

        float time;

        if (speed_squared_difference == 0)
        {
            Debug.Log("Divide by 0");
            //Divide by 0, special case must be treated differently

            time = ET.sqrMagnitude / (2 * enemy_speed * dot_product);

        }
        else
        {
            float discriminant = (float)Math.Pow(enemy_speed * dot_product, 2) - ((float)Math.Pow(enemy_speed, 2) - (float)Math.Pow(projectile_speed, 2)) *  ET.sqrMagnitude;

            if (discriminant < 0)
            {
                //No solutions
                return (null, null);

            }
            else if (discriminant == 0)
            {
                //One solution
                time = enemy_speed * (dot_product) / speed_squared_difference;
            }
            else
            {
                //Two solutions

                time = (enemy_speed * (dot_product) - (float)Math.Sqrt(discriminant)) / speed_squared_difference;

                //Make sure time is positive
                if (time < elapsed_time)
                {
                    time = (enemy_speed * (dot_product) + (float)Math.Sqrt(discriminant)) / speed_squared_difference;
                }
            }
        }

        if (time < elapsed_time)
        {
            return (null, null);
        }

        //Enemy Position + time * Enemy Speed * Enemy Direction = Collision point
        //Collision Point - Tower Position normalised is the projectile Direction

        Vector2 collision_point = enemy_position + time * enemy_speed * enemy_direction;

        return (collision_point, time);
    }


    protected Vector2? aimPrediction(float projectile_speed)
    {
        //See time for enemy to reach destination
        float time_to_destination = (target.GetDestination - target.transform.position).magnitude / target.GetMovementSpeed;

        Vector2 enemy_direction = (target.GetDestination - target.transform.position).normalized;

        (Vector2? collision_point, float? time) point;
        float elapsedTime = 0;
        for (int i = target.getWaypointIndex; i <= target.GetWaypoints.Points.Length - 2; i++) //start at i = index of waypoint enemy is approaching
        {
            if (true)//(i == target.getWaypointIndex) //if first iteration, enemy is heading from current position to index position
            {
                point = GetCollisionPoint(target.transform.position, enemy_direction, target.GetMovementSpeed, projectile_speed, 0);
                elapsedTime += time_to_destination; //increment elapsed time by time taken to get to waypoint
            }
            else
            {
                time_to_destination = (target.GetWaypoints.getWaypointPosition(i + 1) - target.GetWaypoints.getWaypointPosition(i)).magnitude / target.GetMovementSpeed; //time to destination is updated to relect the new distance between points
                enemy_direction = (target.GetWaypoints.getWaypointPosition(i + 1) - target.GetWaypoints.getWaypointPosition(i)).normalized;
                point = GetCollisionPoint(target.GetWaypoints.getWaypointPosition(i), target.GetWaypoints.getWaypointPosition(i + 1), target.GetMovementSpeed, projectile_speed, elapsedTime);
                elapsedTime += time_to_destination;
            }
            
            if (point.collision_point is Vector2 cp)
            {
                //Check if on enemy path
                if (point.time <= time_to_destination)
                {
                    //Fire
                    Vector2 direction = (cp - (Vector2)transform.position).normalized;

                    return direction;
                }
            }
        }

        return null; 
    }

}
