using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
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

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                e.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                e.TakeDamage(damage);
            }
        }
    }
}
