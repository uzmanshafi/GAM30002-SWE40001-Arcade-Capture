using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class DonpachiTower : Tower
{
    private float currentAngle = 0f;

    private Wave wave;
    
    [Header("Level Values Configuration")]
    [SerializeField] private float[] ProjectileSpeedUpgrades;
    [SerializeField] private float[] AngleIncrementUpgrades;

    [SerializeField] private AudioClip shootAudioClip;
    private void Start()
    {
        base.init();
        wave = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Wave>();
    }

    void Update()
    {
        if (wave.waveInProgress)
        {
                tryShoot();
        }
    }

    protected override void tryShoot()
    {
        //Debug.Log("Trying to shoot");
        if (Time.time - lastShotTime > cooldown)
        {
            //AudioSource.PlayClipAtPoint(shootAudioClip, transform.position);
            Shoot(1 << (1 + upgradeLevel));

            lastShotTime = Time.time;
        }
    }

    private void Shoot(int projectileCount)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle + i * (360f / projectileCount));
            Vector2 direction = rotation * Vector2.up;
            ShootProjectile(direction);
        }

        currentAngle += AngleIncrementUpgrades[upgradeLevel];
        if (currentAngle >= 360f) currentAngle -= 360f;
    }

    private void ShootProjectile(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
        projectileRB.velocity = direction * ProjectileSpeedUpgrades[upgradeLevel];

        Projectile proj = bullet.GetComponent<Projectile>();
        proj.target = null;
        proj.damage = damage;

        Destroy(bullet, bulletLifetime);
    }
}