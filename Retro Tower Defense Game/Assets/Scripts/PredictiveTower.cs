using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;
public class PredictiveTower : Tower
{
    [SerializeField] private float projectile_speed;
    [SerializeField] private AudioClip shoot;
    // Start is called before the first frame update
    void Start()
    {
        base.init();
    }

    protected void initialise()
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
        target = furthestTarget();
        if (Time.time - lastShotTime > cooldown && target != null)
        {
            Vector2? dir = aimPrediction(projectile_speed, target, (Vector2)transform.position, range);

            AudioSource.PlayClipAtPoint(shoot, transform.position);
            GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);
            
            Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
            if (dir is Vector2 _dir)
            {
                projectileRB.velocity = _dir * projectile_speed;
            }
            Projectile shot = bullet.GetComponent<Projectile>();
            shot.target = target;
            shot.canSeeCamo = canSeeCamo;
            shot.damage = damage;
            Destroy(bullet, bulletLifetime);
            lastShotTime = Time.time;
        }

    }

    
}
