using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Math;

public class DonpachiTower : Tower
{
    [SerializeField] private float projectileSpeed = 5f;
    private float currentAngle = 0f;
    [SerializeField] private float angleIncrement = 10f;

    private void Start()
    {
        base.init();
    }

    void Update()
    {
        Enemy possibleEnemy = furthestTarget();
        if (possibleEnemy != null)
        {
            tryShoot();
        }
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown)
        {
            Shoot(Math.Pow(2, upgradeLevel));

            lastShotTime = Time.time;
        }
    }

    private void Shoot(int projectileCount) {
        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle + i * (360f / projectileCount));
            Vector2 direction = rotation * Vector2.up;
            ShootProjectile(direction);
        }

        currentAngle += angleIncrement;
        if (currentAngle >= 360f) currentAngle -= 360f;
    }

    private void ShootProjectile(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
        projectileRB.velocity = direction * projectileSpeed;

        bullet.GetComponent<Projectile>().target = null;

        Destroy(bullet, 5f);
    }
}
