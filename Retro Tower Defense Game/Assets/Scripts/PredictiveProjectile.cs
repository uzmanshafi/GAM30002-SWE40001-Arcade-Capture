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

    private void aimPrediction()
    {
        float distance_to_target = Vector2.Distance(transform.position, target.transform.position);
        float time_to_target = distance_to_target / (speed * Time.deltaTime);

        Vector3 movementPerTick = Vector3.MoveTowards(target.transform.position, target.GetDestination, (target.GetMovementSpeed) * Time.deltaTime) - target.transform.position;
        float targetXMovement = movementPerTick.x;
        float targetXPos = target.transform.position.x;
        float xdisplacementPerTick;

        if (targetXMovement > 0) 
        {
            xdisplacementPerTick = targetXPos - (targetXPos + (targetXMovement));
        }
        else
        {
            xdisplacementPerTick = targetXPos - (targetXPos + (targetXMovement));
        }

        float targetYMovement = movementPerTick.y;
        float targetYPos = target.transform.position.y;
        float ydisplacementPerTick;

        if (targetYMovement > 0) 
        {
            ydisplacementPerTick = targetYPos - (targetYPos + (targetYMovement));
        }
        else
        {
            ydisplacementPerTick = targetYPos - (targetYPos + (targetYMovement));
        }

        float xDisplacement = targetXMovement * time_to_target;
        float yDisplacement = targetYMovement * time_to_target;
        Vector2 predictedVec = new Vector2(targetXPos + xDisplacement, targetYPos + yDisplacement); //predict location based on time for projectile to reach the target

        float dx = predictedVec.x - transform.position.x;
        float dy = predictedVec.y - transform.position.y;

        double firing_angle = Math.Atan2(dy, dx); //using the displacmement, Find the angle we will have to fire at from our current pos.

        Vector2 bulletVel = new Vector2((float)(speed * Math.Cos(firing_angle)), (float)(speed * Math.Sin(firing_angle))); //Utilising the speed and firing angle, create a vector for the new bullet's required velocity

        //transform.LookAt(target.transform);
        rb.velocity = bulletVel;

    }

}
