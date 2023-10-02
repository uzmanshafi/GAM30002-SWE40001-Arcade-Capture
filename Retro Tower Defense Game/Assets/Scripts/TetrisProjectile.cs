using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisProjectile : Projectile
{

    public TetrisTower owner; // This is instantiated on creation
    [SerializeField] private AudioClip hit;

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
                float damageMultiplier = owner.enemyHitEvent(e);
                AudioSource.PlayClipAtPoint(hit, transform.position);
                e.TakeDamage(damage * damageMultiplier);
                Destroy(gameObject);
            }
        }
    }

    // What is the difference between this and OnCollisionEnter2D?
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
