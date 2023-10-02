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
	[SerializeField] private AudioClip shoot;

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
			TetrisProjectile projectile = bulletTypes[bulletType].GetComponent<TetrisProjectile>();
			Vector2? dir = aimPrediction(projectile.speed, target, (Vector2)transform.position, range);
			if (dir is Vector2 _dir)
			{
				AudioSource.PlayClipAtPoint(shoot, transform.position);
				GameObject bullet = Instantiate(bulletTypes[bulletType], transform.position /*+ dir*/, Quaternion.identity);
				projectile = bullet.GetComponent<TetrisProjectile>();
				projectile.owner = this;
				bullet.GetComponent<Rigidbody2D>().velocity = _dir * projectile.speed;
				Destroy(bullet, bulletLifetime);
				lastShotTime = Time.time;
			}
			
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
