using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

// Design A
// This tower will shoot projectiles that send back an onHit event. If they do hit something then they must check if it's the same enemy as lastEnemyHit. If it is increase hitCount and deal damage somehow. Else reset hitCount and update lastEnemyHit

// Design B
// Keep track of all hit enemies and how many times. Each update check that the enemy exists in the global array. If it doesn't then remove it from self.


public class TetrisTower : Tower
{

	private Enemy? lastEnemyHit;
	private int hitCount;

	/*
	Hits 	|	Damage Multiplier:
	1		|	1
	2		|	comboMultiplier
	3		|	comboMultiplier * 2
	4		|	combotMultiplier * 3
			...
	*/
	[SerializeField] private float comboMultiplier;

	// Start is called before the first frame update
	void Start()
	{
		base.init();
		lastEnemyHit = null;
		hitCount = 0;
		loadUpgrades();
		
	}

	private void loadUpgrades()
    {
		TowerUpgrade upgrade1 = new TowerUpgrade();
		TowerUpgrade upgrade2 = new TowerUpgrade();
		TowerUpgrade upgrade3 = new TowerUpgrade();

		upgrade1.upgradeLevel = UpgradeLevel.Lvl1;
		upgrade1.cost = cost;
		upgrade1.description = "An arcade that builds a combo on Players that are hit consecutivley";

		upgrade2.upgradeLevel = UpgradeLevel.Lvl2;
		upgrade2.cost = cost;
		upgrade2.description = "The arcade's cooldown is reduced and can build combos quicker";

		upgrade3.upgradeLevel = UpgradeLevel.Lvl3;
		upgrade3.cost = cost * 2;
		upgrade3.description = "The arcade's cooldown is reduced futher leading to lightning fast combos";

		upgrades[0] = upgrade1;
		upgrades[1] = upgrade2;
		upgrades[2] = upgrade3;
	}

	// Update is called once per frame
	void Update()
	{
		tryShoot();
	}

	protected override void tryShoot()
	{
		target = furthestTarget();
		if (Time.time - lastShotTime > cooldown && target != null)
		{

			int bulletType = Random.Range(0,6);

			GameObject bullet = Instantiate(bulletTypes[bulletType], transform.position /*+ dir*/, Quaternion.identity);
			TetrisProjectile projectile = bullet.GetComponent<TetrisProjectile>();
			projectile.owner = this;
			Vector2? dir = aimPrediction(projectile.speed, target, (Vector2)transform.position);
			if (dir is Vector2 _dir)
			{
				bullet.GetComponent<Rigidbody2D>().velocity = _dir * projectile.speed;
			}
			Destroy(bullet, bulletLifetime);
			lastShotTime = Time.time;
		}

	}

	// This function is called by the projectile and it returns the damage multiplier.
	public float enemyHitEvent(Enemy enemy) {
		if (lastEnemyHit == enemy) {
			hitCount++;
			return comboMultiplier * (hitCount - 1); 
		} else {
			lastEnemyHit = enemy;
			hitCount = 1;
			return 1;
		}
	}

}
