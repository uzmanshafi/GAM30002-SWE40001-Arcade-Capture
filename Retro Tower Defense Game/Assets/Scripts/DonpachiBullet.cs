using System.Collections;
using UnityEngine;

public class DonpachiBullet : Projectile
{
    public DonpachiTower owner; // This is instantiated on creation

    // Time to live in seconds
    public float timeToLive = 1.5f;


    void Start()
    {
        Debug.Log("DonpachiBullet Start method called."); 
        Destroy(gameObject, timeToLive);
    }
    
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
            e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            e.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
