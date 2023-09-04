using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvadersTower : Tower
{
    [SeralizeField] private float projectile_speed;
    private float currentPos;
    private float leftBound;
    private float rightBound;

    
    void start()
    {

    }

    void update()
    {
        tryShoot();
    }

    private void tryShoot()
    {
        target = furthestTarget();
        if (Time.time - lastShotTime > cooldown && target != null)
        {
            Vector2? dir = aimPrediction(projectile_speed);
            
            
            GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);
            Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
            if (dir is Vector2 _dir)
            {
                projectileRB.velocity = _dir * projectile_speed;
            }
           
            lastShotTime = Time.time;
        }
    }

    private void MoveOnASlider()
    {

    }

    // notes: extra collider on the bottom of the tower to move it left and right while making
    // sure it doesn't go on a tile path. should be rotatable as well.
    // other towers should not be allowed to place on top of this tower and its slider collider.
    // the tower should be able to shoot straight up and move left and right.
}