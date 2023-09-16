using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonpachiTower : Tower
{
    [SerializeField] private float projectileSpeed = 5f;
    private float currentAngle = 0f;  // Initial angle
    [SerializeField] private float angleIncrement = 10f; // Angle increment for the spiral pattern

    void Update()
    {
        tryShoot();
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown)
        {
     
            Quaternion rotation1 = Quaternion.Euler(0, 0, currentAngle);
            Vector2 direction1 = rotation1 * Vector2.up;

            Quaternion rotation2 = Quaternion.Euler(0, 0, currentAngle + 180f);
            Vector2 direction2 = rotation2 * Vector2.up; 


            ShootProjectile(direction1);

            ShootProjectile(direction2);

            currentAngle += angleIncrement;

            if (currentAngle >= 360f)
            {
                currentAngle -= 360f;
            }

            lastShotTime = Time.time;
        }
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
