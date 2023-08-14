using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile
{

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
        closestTarget();
        direction = (position - (Vector2)target.transform.position).normalized;
        position = direction * speed;

    }
    private void closestTarget()
    {
        Enemy[] allTargets = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);
        float bestDist = 0;
        float tempDist;
        foreach (Enemy e in allTargets)
        {
            tempDist = Vector2.Distance(position, e.transform.position);
            if (tempDist > bestDist)
            {
                bestDist = tempDist;
                target = e;
            }
        }
    }

}
