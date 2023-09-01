using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tryShoot();
    }

    protected override void tryShoot() //abstract?
    {
        furthestTarget();
        if (Time.time - lastShotTime > cooldown && target != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            
            
            
            
            GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);
            Projectile projectile = bullet.GetComponent<Projectile>();
            projectile.target = target;
            lastShotTime = Time.time;
        }

    }

    
}