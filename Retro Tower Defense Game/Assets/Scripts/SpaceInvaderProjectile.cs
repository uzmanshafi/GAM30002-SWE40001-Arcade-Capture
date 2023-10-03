using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvaderProjectile : Projectile
{
    void Update()
    {
        move();
    }

    public override void move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                e.TakeDamage(damage);
                Destroy(gameObject);
                Debug.Log("SpaceInvader Bullet collided with enemy.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) 
            {
                e.TakeDamage(damage);
                Destroy(gameObject); 
            }
        }
    }
}
