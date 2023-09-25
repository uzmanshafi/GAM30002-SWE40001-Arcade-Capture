using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsTower : Tower
{
    [SerializeField] private float asteroidSpeed;
    // Start is called before the first frame update
    void Start()
    {
        base.init();
        loadUpgrades();
    }

    private void loadUpgrades()
    {
        TowerUpgrade upgrade1 = new TowerUpgrade();
        TowerUpgrade upgrade2 = new TowerUpgrade();
        TowerUpgrade upgrade3 = new TowerUpgrade();

        upgrade1.upgradeLevel = UpgradeLevel.Lvl1;
        upgrade1.cost = cost;
        upgrade1.description = "An arcade that calls in an astroid from off space to strike your target";

        upgrade2.upgradeLevel = UpgradeLevel.Lvl2;
        upgrade2.cost = cost;
        upgrade2.description = "Calls in 2 asteroids to coverge on your target";

        upgrade3.upgradeLevel = UpgradeLevel.Lvl3;
        upgrade3.cost = cost * 2;
        upgrade3.description = "Calls in 3 asteroids to coverge on your target";

        upgrades[0] = upgrade1;
        upgrades[1] = upgrade2;
        upgrades[2] = upgrade3;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShotTime > cooldown && GameManager.instance.AllEnemies(this.canSeeCamo).Count != 0)
        {
            tryShoot();
            lastShotTime = Time.time;
        }
    }

    protected override void tryShoot()
    {
        /*
        float spawnX = 0;
        float spawnY = 0;
        int choice = Random.Range(0, 3);
        if (choice == 0) // spawn quadrant for asteroid: 0 = left, 1 = right, 2 = up, 3 = down
        {
            spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y);
            spawnX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.mainWindowPosition.x, 0)).x - 1;
        }
        else if (choice == 1)
        {
            spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y);
            spawnX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.mainWindowPosition.x + Screen.width, 0)).x + 1;
        }
        else if (choice == 2)
        {
            spawnY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y + 1;
            spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x + Screen.width)).x);
        }
        else if (choice == 3)
        {
            spawnY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y - 1;
            spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x + Screen.width)).x);
        }*/


        GameObject asteroid = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - asteroid.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
        asteroid.GetComponent<Projectile>().damage = damage;
        Destroy(asteroid, bulletLifetime);
    }
}
