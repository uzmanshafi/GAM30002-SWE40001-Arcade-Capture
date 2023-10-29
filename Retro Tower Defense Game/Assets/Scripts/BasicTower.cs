using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTower : Tower
{
    // Start is called before the first frame update
    void Start()
    {
        base.init();
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
            
            
            
            
            GameObject bullet = Instantiate(bulletTypes[0], transform.position + dir, Quaternion.identity);
            Projectile projectile = bullet.GetComponent<Projectile>();
            projectile.target = target;
            projectile.canSeeCamo = canSeeCamo;
            projectile.canSeeCamo2 = isStaffBuffed2;
            Destroy(projectile.gameObject, bulletLifetime);
            lastShotTime = Time.time;
        }

    }

    
}
