using UnityEngine;
using System;
using System.Collections.Generic;
public static class Utils {

    //Add the time returned to the elapsed time to get total time
    private static (Vector2? collision_point, float? time) GetCollisionPoint(Vector2 enemy_position, Vector2 enemy_direction, float enemy_speed, Vector2 current_position, float projectile_speed, float elapsed_time)
    {
        //The vector from Enemy to Tower
        Vector2 ET = current_position - enemy_position;

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

    private static float TimeToDestination(Vector2 current_position, Vector2 destination_position, float speed)
    {
        return (destination_position - current_position).magnitude / speed;
    }

    private static (Vector2? direction, float time_to_destination) AttemptToFire(Vector2 enemy_position, Vector2 enemy_destination, float enemy_speed, Vector2 tower_position, float projectile_speed, float elapsed_time)
    {
        Vector2 enemy_direction = (enemy_destination - enemy_position).normalized;
        (Vector2? collision_point, float? time) point = GetCollisionPoint(enemy_position, enemy_direction, enemy_speed, tower_position, projectile_speed, elapsed_time);

        float time_to_destination = TimeToDestination(enemy_position, enemy_destination, enemy_speed);
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

    public static Vector2? aimPrediction(float projectile_speed, Enemy target, Vector2 current_position, float tower_range)
    {

        //It takes in the current enemy position and then goes from there
        // target.transform.position;
        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex);
        // 0;

        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex);
        // target.GetWaypoints.getWaypointPosition(target.getWaypointIndex + 1)
        // elapsed_time;
        float total_time = 0;

        (Vector2? direction, float time) attempt = AttemptToFire(target.transform.position, target.GetDestination, target.GetMovementSpeed, current_position, projectile_speed, total_time);


        if (attempt.direction is Vector2 dir)
        {
            return dir;
        } else {
            for (int index = target.getWaypointIndex; index < target.GetWaypoints.Points.Length - 1; ++index)
            {
                total_time += attempt.time;

                attempt = AttemptToFire(target.GetWaypoints.getWaypointPosition(index), target.GetWaypoints.getWaypointPosition(index + 1), target.GetMovementSpeed, current_position, projectile_speed, total_time);

                if (attempt.direction is Vector2 dir2 && projectile_speed * total_time <= tower_range)
                {
                    return dir2;
                }
            }
        }

        return null; 
    }

	public static bool pointReached(Vector2 position, Vector2 destination, float threshold = 0.01f)
    {
        float distanceToNext = (position - destination).sqrMagnitude;
        return distanceToNext <= threshold;
    }

    public static bool withinRange(Vector2 pos1, Vector2 pos2, float range) {
        return (pos1 - pos2).sqrMagnitude <= Math.Pow(range, 2);
    }

    public static GameObject? nearestEnemy(Vector2 position, GameObject currentTarget,List<GameObject> enemies) {
        GameObject? closest = null;
        foreach (GameObject enemy in enemies) {
            if (closest == null && enemy != currentTarget) {
                closest = enemy;
            } else {
                if (enemy != currentTarget && (position - (Vector2)enemy.transform.position).sqrMagnitude < (position - (Vector2)closest.transform.position).sqrMagnitude) {
                    closest = enemy;
                }
            }
        }
        return closest;
    }

}