using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : Projectile
{

    //public Enemy target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkTarget();
        move();
        
    }

    private void checkTarget()
    {
        if (target == null)
        {
            closestTarget();
        }
    }

    public override void move()
    {
        
        direction = (target.transform.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed) * Time.deltaTime;

    }
    private void closestTarget()
    {
        List<Enemy> allTargets = GameManager.instance.AllEnemies(canSeeCamo);
        float bestDist = Mathf.Infinity;
        float tempDist;
        foreach (Enemy e in allTargets)
        {
            tempDist = Vector2.Distance(position, e.transform.position); //Get closest enemy to the projectile
            if (tempDist < bestDist)
            {
                bestDist = tempDist;
                target = e;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                //e.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }

}
