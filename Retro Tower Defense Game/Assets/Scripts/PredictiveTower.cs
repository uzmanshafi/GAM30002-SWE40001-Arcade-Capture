using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Utils;
public class PredictiveTower : Tower
{
    [SerializeField] protected float projectile_speed;
    [SerializeField] protected AudioClip shoot;
    [SerializeField] private AudioMixerGroup soundGroup;
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

            
            if (dir is Vector2 _dir)
            {
                SoundEffect.PlaySoundEffect(shoot, transform.position, 1, soundGroup);
                GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);

                Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
                projectileRB.velocity = _dir * projectile_speed;

                Projectile shot = bullet.GetComponent<Projectile>();
                shot.target = target;
                shot.canSeeCamo = canSeeCamo;
                shot.canSeeCamo2 = isStaffBuffed2;
                shot.damage = damage;
                Destroy(bullet, bulletLifetime);
                lastShotTime = Time.time;
            }
            
        }

    }

    
}
