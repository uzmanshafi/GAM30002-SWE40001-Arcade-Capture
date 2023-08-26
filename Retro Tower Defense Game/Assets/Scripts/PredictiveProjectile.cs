using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PredictiveProjectile : Projectile
{

    //public Enemy target;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        aimPrediction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void move()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            e.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    //Returns the collision point and time of collision of the predicted shot. This only works for rays, it does not account for corners. Use the returned point to check if it lies on the enemy path. elapsed_time is the time that it took the enemy to reach enemy_position
    private (Vector2? collision_point, float? time) GetCollisionPoint(Vector2 enemy_position, Vector2 enemy_direction, float enemy_speed, float elapsed_time)
    {
        //The vector from Enemy to Tower
        Vector2 ET = transform.position - enemy_position;

        float dot_product = Vector2.Dot(enemy_direction, ET);

        float speed_squared_difference = Math.pow(enemy_speed, 2) - Math.pow(transform.speed, 2);

        float time;

        if (speed_squared_difference == 0 ) {
            //Divide by 0, special case must be treated differently

            time = ET.sqrMagnitude / (2 * enemy_speed * dot_product);

        } else {
            float discriminant = Math.pow(enemy_speed * dot_product, 2) * ET.sqrMagnitude;

            if (discriminant < 0) {
                //No solutions
                return (null, null);

            } else if (discriminant == 0) {
                //One solution
                time = enemy_speed * (dot_product) / speed_squared_difference;
            } else {
                //Two solutions

                time = (enemy_speed * (dot_product) - Math.Sqrt(discriminant)) / speed_squared_difference;

                //Make sure time is positive
                if (time < elapsed_time) {
                    time = (enemy_speed * (dot_product) + Math.Sqrt(discriminant)) / speed_squared_difference;
                }
            }
        }

        if (time < elapsed_time) {
            return (null, null);
        }

        //Enemy Position + time * Enemy Speed * Enemy Direction = Collision point
        //Collision Point - Tower Position normalised is the projectile Direction

        Vector2 collision_point = enemy_position + time * enemy_speed * enemy_direction;

        return (collision_point, time);
    }


    private void aimPrediction()
    {
        //See time for enemy to reach destination
        float time_to_destination = (target.GetDestination - target.transform.position).magnitude / target.GetMovementSpeed;

        Vector2 enemy_direction = (target.GetDestination - target.transform.position).normalized;

        var point = GetCollisionPoint(target.transform.position, enemy_direction, target.GetMovementSpeed, 0);

        if (point.collision_point is Vector2 cp) {
            //Check if on enemy path
            if (time_to_destination <= point.time)
            {
                //Fire
                //transform.direction = (collision_point - transform.position).normalized;
                return;
            }
        }

        //aimPrediction()
        //Repeat but update enemy position and enemy direction based on next waypoints. GetCollisionPoint elapsed time is time_to_destination. If enemy escapes (last waypoint) then return



    }

}
