using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Utils;

// This will hit the nearest enemy. Once it hits it it will bounce towards the next enemy and repeat

public class BreakoutProjectile : Projectile
{
    // This is how many enemies it can hit before it is destroyed
    [SerializeField] private int pierce;

    // Start is called before the first frame update
    void Start()
    {
        // destination = target.transform.position;
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


    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            e.TakeDamage(damage);
            pierce--;
            if (pierce < 0)
            {
                Destroy(gameObject);
            } else {
                //Go towards nearest enemy with predictive magic
                //The AllEnemies array must be accessible

                GameObject? nearest = nearestEnemy(transform.position, e.gameObject, GameObject.FindAnyObjectByType<GameManager>().AllEnemies(canSeeCamo));

                if (nearest is GameObject _nearest) {
                    Vector2? dir = aimPrediction(speed, _nearest.GetComponent<Enemy>(), (Vector2)transform.position);
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
