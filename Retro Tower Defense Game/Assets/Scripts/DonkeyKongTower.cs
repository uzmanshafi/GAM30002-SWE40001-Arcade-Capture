using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class DonkeyKongTower : PredictiveTower
{
	private int currentLevel;

	private Vector2 targetPosition;
	private int targetIndex;

	private Waypoints currentWaypoints;

	private Wave waveManager;
	// Start is called before the first frame update
	void Start()
    {
        base.init();
		currentLevel = upgradeLevel;

		waveManager = GameManager.instance.GetComponent<Wave>();
		// Find closest point on path

		float nearest_point_distance_squared = float.MaxValue;
		Vector2? nearest_point = null;
		int tempIndex = 0;
		currentWaypoints = GameManager.instance.GetComponent<Waypoints>();

		// Loop through waypoints
		for (int i = 0; i < currentWaypoints.Points.Length - 1; ++i) {
			// For each waypoint segment, get nearest point from tower position.
			Vector2 point = nearestPointOnSegment(this.transform.position, currentWaypoints.Points[i], currentWaypoints.Points[i+1]);
			// If distance is smaller than nearest_point_distance then update it as well as nearest_point
			if (((Vector2)this.transform.position - point).sqrMagnitude < nearest_point_distance_squared) {
				nearest_point = point;
				tempIndex = i;
				nearest_point_distance_squared = ((Vector2)this.transform.position - point).sqrMagnitude;
			}
		}
		if (nearest_point is Vector2 pt)
        {
			targetPosition = pt;
			targetIndex = tempIndex;
		}
		//Set the targetting position to be nearest_point instead of using predictive__projectile

	}


    // Update is called once per frame
    void Update()
    {
		tryShoot();
        checkUpgrades();
    }

	protected override void tryShoot()
    {
		if (targetPosition != null)
        {
			if (Time.time - lastShotTime > cooldown && waveManager.waveInProgress)
			{
				Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;


				AudioSource.PlayClipAtPoint(shoot, transform.position);

				GameObject bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
				DonkeyKongProjectile projectile = bullet.GetComponent<DonkeyKongProjectile>();
				projectile.pathPoints = currentWaypoints.Points;
				projectile.pathPointIndex = targetIndex;
				projectile.canSeeCamo = canSeeCamo;
				projectile.canSeeCamo2 = isStaffBuffed2;
				projectile.damage = damage;
				bullet.GetComponent<Rigidbody2D>().velocity = dir * projectile.speed;
				Destroy(projectile.gameObject, bulletLifetime);
				lastShotTime = Time.time;
			}
		}
        else
        {
			target = furthestTarget();
			if (Time.time - lastShotTime > cooldown && target != null)
			{
				Vector2? dir = aimPrediction(projectile_speed, target, (Vector2)transform.position, range);


				if (dir is Vector2 _dir)
				{
					AudioSource.PlayClipAtPoint(shoot, transform.position);
					GameObject bullet = Instantiate(bulletTypes[0], transform.position /*+ dir*/, Quaternion.identity);

					Rigidbody2D projectileRB = bullet.GetComponent<Rigidbody2D>();
					projectileRB.velocity = _dir * projectile_speed;

					Projectile shot = bullet.GetComponent<Projectile>();
					shot.target = target;
					shot.canSeeCamo = canSeeCamo;
					shot.damage = damage;
					Destroy(bullet, bulletLifetime);
					lastShotTime = Time.time;
				}

			}
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
