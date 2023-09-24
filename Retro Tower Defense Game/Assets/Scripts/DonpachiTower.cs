using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonpachiTower : Tower
{
    [SerializeField] private float projectileSpeed = 5f;
    private float currentAngle = 0f;
    [SerializeField] private float angleIncrement = 10f;

    public bool isLevel1 = true;
    public bool isLevel2 = false;
    public bool isLevel3 = false;

    void OnValidate()
    {
        if (isLevel1)
        {
            isLevel2 = false;
            isLevel3 = false;
        }
        else if (isLevel2)
        {
            isLevel1 = false;
            isLevel3 = false;
        }
        else if (isLevel3)
        {
            isLevel1 = false;
            isLevel2 = false;
        }
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
            if (isLevel1)
            {
                Level1Shoot();
            }
            else if (isLevel2)
            {
                Level2Shoot();
            }
            else if (isLevel3)
            {
                Level3Shoot();
            }

            lastShotTime = Time.time;
        }
    }

    private void Level1Shoot()
    {
        Quaternion rotation1 = Quaternion.Euler(0, 0, currentAngle);
        Vector2 direction1 = rotation1 * Vector2.up;

        Quaternion rotation2 = Quaternion.Euler(0, 0, currentAngle + 180f);
        Vector2 direction2 = rotation2 * Vector2.up;

        ShootProjectile(direction1);
        ShootProjectile(direction2);

        currentAngle += angleIncrement;
        if (currentAngle >= 360f) currentAngle -= 360f;
    }

    private void Level2Shoot()
    {
        for (int i = 0; i < 4; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle + i * 90f);
            Vector2 direction = rotation * Vector2.up;
            ShootProjectile(direction);
        }

        currentAngle += angleIncrement;
        if (currentAngle >= 360f) currentAngle -= 360f;
    }

    private void Level3Shoot()
    {
        for (int i = 0; i < 8; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle + i * 45f);
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
