using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkeyKongTower : PredictiveTower
{
	private int currentLevel;
	private Vector2 nearestPathDir;


	// Pass in all waypoints
	Vector2 nearestPoint(Vector2[] points) {
		Vector2? nearestPoint = null;
		float nearestDistance = Math.Infinity;

		foreach (var point in points) {
			if ((transform.position - point).sqrMagnitude < nearestDistance) {
				nearestPoint = point;
			}	
		}

		if (nearestPoint == null) {
			throw error;
		}

		return nearestPoint;
	}

    // Start is called before the first frame update
    void Start()
    {
        base.initialise();
		currentLevel = upgradeLevel;
		nearestPathDir = (nearestPoint(allWaypoints) - transform.position).normalized;
	}


    // Update is called once per frame
    void Update()
    {
        tryShoot();
        checkUpgrades();
    }

	// Shoot towards nearest path
	protected override void tryShoot() {
		target = furthestTarget();
        if (Time.time - lastShotTime > cooldown/* && target != null*/)
        {
			AudioSource.PlayClipAtPoint(shoot, transform.position);
			GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);

			Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
			projectileRB.velocity = nearestPathDir * projectile_speed;

			Projectile shot = bullet.GetComponent<Projectile>();
			shot.target = target;
			shot.canSeeCamo = canSeeCamo;
			shot.damage = damage;
			Destroy(bullet, bulletLifetime);
			lastShotTime = Time.time;
		}
	}

	private void checkUpgrades()
	{
		if (currentLevel != upgradeLevel)
		{
			if (upgradeLevel == 1)
			{
				base_cooldown = base_cooldown * 0.85f;
			}
			if (upgradeLevel == 2)
			{
				base_cooldown = base_cooldown * 0.65f;
			}
			currentLevel = upgradeLevel;
		}
	}


}
