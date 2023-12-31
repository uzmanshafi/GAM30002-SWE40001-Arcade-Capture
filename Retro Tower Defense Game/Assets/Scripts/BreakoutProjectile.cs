using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using static Utils;

// This will hit the nearest enemy. Once it hits it it will bounce towards the next enemy and repeat

public class BreakoutProjectile : Projectile
{
    // This is how many enemies it can hit before it is destroyed
    [SerializeField] private int pierce;
    [SerializeField] private AudioClip rebound;
    [SerializeField] private AudioMixerGroup soundGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame. (Does this call move?)
    void Update()
    {
        move();
    }

    public override void move()
    {
        //Go towards destination. Note that we update destination if rollingAlongPath
        //transform.position += (Vector3)(direction * speed) * Time.deltaTime;
        
    }


    private Enemy? nearestEnemy(Vector2 position, Enemy currentTarget ,List<Enemy> enemies) {
        Enemy? closest = null;
        foreach (Enemy enemy in enemies) {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                e.TakeDamage(damage);
                pierce--;
                SoundEffect.PlaySoundEffect(rebound, transform.position, 1, soundGroup);
                if (pierce < 0)
                {
                    Destroy(gameObject);
                } else {
                    //Go towards nearest enemy with predictive magic
                    //The AllEnemies array must be accessible

                    Enemy? nearest = nearestEnemy(transform.position, e, GameManager.instance.AllEnemies(canSeeCamo));

                    if (nearest is Enemy _nearest) {
                        Vector2? dir = aimPrediction(speed, _nearest, (Vector2)transform.position, Mathf.Infinity);
                        if (dir is Vector2 _dir) {
                            direction = _dir;
                            float angle = Mathf.Atan2(_dir.y, _dir.x);
                            _dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                            gameObject.GetComponent<Rigidbody2D>().velocity = _dir * speed;

                        }
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
