using UnityEngine;

public class DonpachiTower : Tower
{
    [Header("Level 1 Configuration")]
    [SerializeField] private float level1ProjectileSpeed = 5f;
    [SerializeField] private float level1AngleIncrement = 10f;

    [Header("Level 2 Configuration")]
    [SerializeField] private float level2ProjectileSpeed = 7f;
    [SerializeField] private float level2AngleIncrement = 15f;

    [Header("Level 3 Configuration")]
    [SerializeField] private float level3ProjectileSpeed = 10f;
    [SerializeField] private float level3AngleIncrement = 20f;

    [Header("Temp Levels Configuration")]
    public bool isLevel1 = true;
    public bool isLevel2 = false;
    public bool isLevel3 = false;

    private float currentAngle = 0f;
    private float projectileSpeed;
    private float angleIncrement;
    private Wave wave;
    [SerializeField] private AudioClip shoot;

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

    void Start()
    {
        base.init();
        wave = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Wave>();
        ConfigureTower();
    }

    void ConfigureTower()
    {
        if (isLevel1)
        {
            projectileSpeed = level1ProjectileSpeed;
            angleIncrement = level1AngleIncrement;
        }
        else if (isLevel2)
        {
            projectileSpeed = level2ProjectileSpeed;
            angleIncrement = level2AngleIncrement;
        }
        else if (isLevel3)
        {
            projectileSpeed = level3ProjectileSpeed;
            angleIncrement = level3AngleIncrement;
        }
    }

    void Update()
    {
        if (wave.waveInProgress)
        {
            Enemy possibleEnemy = furthestTarget();
            if (possibleEnemy != null)
            {
                tryShoot();
            }
        }
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown)
        {
            AudioSource.PlayClipAtPoint(shoot, transform.position);
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

        Projectile proj = bullet.GetComponent<Projectile>();
        proj.target = null;
        proj.damage = damage;

        Destroy(bullet, bulletLifetime);
    }
}
