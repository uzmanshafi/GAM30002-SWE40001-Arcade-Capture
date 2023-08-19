using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictiveProjectile : Projectile
{
    [SerializeField] private float speed;

    public Enemy target;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void move()
    {
        
    }

    private void aimPrediction()
    {/*
        float distance_to_target = Vector3.Distance(transform.position, target.transform.position);
        float time_to_target = distance_to_target / speed;
        float targetXMovement = target.velocity.x;
        float targetXPos = player.transform.position.x;
        float xdisplacementPerTick;

        if (targetXMovement > 0) //Find player's X movement/displacement
        {
            xdisplacementPerTick = targetXPos - (targetXPos + (targetXMovement * Time.deltaTime));
        }
        else
        {
            xdisplacementPerTick = targetXPos - (targetXPos - (targetXMovement * Time.deltaTime));
        }

        float targetYMovement = playerRB.velocity.y;
        float targetYPos = player.transform.position.y;
        float ydisplacementPerTick;

        if (targetYMovement > 0) //Find player's Y movement/displacement
        {
            ydisplacementPerTick = targetYPos - (targetYPos + (targetYMovement * Time.deltaTime));
        }
        else
        {
            ydisplacementPerTick = targetYPos - (targetYPos - (targetYMovement * Time.deltaTime));
        }

        float xDisplacement = xdisplacementPerTick * time_to_target;
        float yDisplacement = ydisplacementPerTick * time_to_target;
        Vector3 predictedVec = new Vector3(targetXPos + xDisplacement, targetYPos + yDisplacement); //predict location based on time for projectile to reach the target

        float dx = predictedVec.x - transform.position.x;
        float dy = predictedVec.y - transform.position.y;

        double firing_angle = Math.Atan2(dy, dx); //using the displacmement, Find the angle we will have to fire at from our current pos.

        Vector3 bulletVel = new Vector3((float)(speed * Math.Cos(firing_angle)), (float)(speed * Math.Sin(firing_angle))); //Utilising the speed and firing angle, create a vector for the new bullet's required velocity

        GameObject shotInstance = Instantiate(shot, new Vector3(shotSP.position.x, shotSP.position.y), Quaternion.identity);
        shotInstance.transform.LookAt(player.transform);
        shotInstance.GetComponent<Rigidbody>().velocity = bulletVel;
        lastShot = Time.time;
*/
    }

}
