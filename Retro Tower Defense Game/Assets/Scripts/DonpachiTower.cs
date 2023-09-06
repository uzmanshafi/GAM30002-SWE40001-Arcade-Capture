using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonpachiTower : Tower
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    void Update()
    {
        transform.Rotate(Vector3.forward, 360 * Time.deltaTime);

    }

    protected override void tryShoot()
    {
        ShootProjectile();
        lastShotTime = Time.time;
    }


    void ShootProjectile()
    {
        
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        Vector2 shootDirection = transform.up;
        rb.velocity = shootDirection * projectileSpeed;
    }
}
