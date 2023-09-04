using System;
using UnityEngine;
using System.Collections;

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
        Enemy bestEnemy = null;
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
                    bestEnemy = e;
                }
            }
            return bestEnemy;
        }
        return null;
    }

    //Add the time returned to the elapsed time to get total time
    private (Vector2? collision_point, float? time) GetCollisionPoint(Vector2 enemy_position, Vector2 enemy_direction, float enemy_speed, float projectile_speed, float elapsed_time)
    {
        //The vector from Enemy to Tower
        Vector2 ET = (Vector2)transform.position - enemy_position;

        float dot_product = Vector2.Dot(enemy_direction, ET);

        float enemy_speed_squared = (float)Math.Pow(enemy_speed, 2);
        float projectile_speed_squared = (float)Math.Pow(projectile_speed, 2);
        float speed_squared_difference = enemy_speed_squared - projectile_speed_squared;


        float time;

        if (speed_squared_difference == 0)
        {
            Debug.Log("Divide by 0");
            //Divide by 0, special case must be treated differently

            //time = ET.sqrMagnitude / (2 * enemy_speed * dot_product);
            time = (ET.sqrMagnitude - (float)Math.Pow(elapsed_time, 2) * projectile_speed_squared) / (2 * (elapsed_time * projectile_speed_squared + enemy_speed * dot_product));
        }
        else
        {
            float discriminant = (float)Math.Pow(elapsed_time * projectile_speed_squared + enemy_speed * dot_product, 2) - speed_squared_difference * (ET.sqrMagnitude - (float)Math.Pow(elapsed_time, 2) * projectile_speed_squared);

            if (discriminant < 0)
            {
                //No solutions
                return (null, null);

            }
            else if (discriminant == 0)
            {
                //One solution
                time = (elapsed_time * projectile_speed_squared + enemy_speed * (dot_product)) / speed_squared_difference;
            }
            else
            {
                //Two solutions

                time = (elapsed_time * projectile_speed_squared + enemy_speed * (dot_product) - (float)Math.Sqrt(discriminant)) / speed_squared_difference;

                //Make sure time is positive
                if (time < 0)
                {
                    time = (elapsed_time * projectile_speed_squared + enemy_speed * (dot_product) + (float)Math.Sqrt(discriminant)) / speed_squared_difference;
                }
            }
        }

        if (time < 0)
        {
            return (null, null);
        }

        //Enemy Position + time * Enemy Speed * Enemy Direction = Collision point
        //Collision Point - Tower Position normalised is the projectile Direction

        Vector2 collision_point = enemy_position + time * enemy_speed * enemy_direction;

        return (collision_point, time);
    }

    private float TimeToDestination(Vector2 current_position, Vector2 destination_position, float speed)
    {
        return (destination_position - current_position).magnitude / speed;
    }

    private (Vector2? direction, float time_to_destination) AttemptToFire(Vector2 enemy_position, Vector2 enemy_destination, float enemy_speed, Vector2 tower_position, float projectile_speed, float elapsed_time)
    {
        Vector2 enemy_direction = (enemy_destination - enemy_position).normalized;
        (Vector2? collision_point, float? time) point = GetCollisionPoint(enemy_position, enemy_direction, enemy_speed, projectile_speed, elapsed_time);

        float time_to_destination = TimeToDestination(enemy_position, enemy_destination, target.GetMovementSpeed);
        if (point.collision_point is Vector2 cp)
        {
            if (point.time <= time_to_destination)
            {
                //Fire
                Vector2 direction = (cp - tower_position).normalized;
                return (direction, time_to_destination);
            }
        }
        return (null, time_to_destination);
    }

    protected Vector2? aimPrediction(float projectile_speed)
    {

        //It takes in the current enemy position and then goes from there
        // target.transform.position;
        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex);
        // 0;

        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex);
        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex + 1)
        // elapsed_time;
        float total_time = 0;

        (Vector2? direction, float time) attempt = AttemptToFire(target.transform.position, target.GetDestination, target.GetMovementSpeed, (Vector2)transform.position, projectile_speed, total_time);


        if (attempt.direction is Vector2 dir)
        {
            return dir;
        } else {
            for (int index = target.getWaypointIndex; index < target.GetWaypoints.Points.Length - 1; ++index)
            {
                total_time += attempt.time;

                attempt = AttemptToFire(target.GetWaypoints.getWaypointPosition(index), target.GetWaypoints.getWaypointPosition(index + 1), target.GetMovementSpeed, (Vector2)transform.position, projectile_speed, total_time);

                if (attempt.direction is Vector2 dir2)
                {
                    return dir2;
                }
            }
        }

        return null; 
    }
    
    // Draws the range radius of a tower when clicked
    private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, range);
	}
}
