using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetFighterTower : BasicTower
{
	private int currentLevel;
	[SerializeField] private AudioClip shoot;
	// Start is called before the first frame update
	void Start()
    {
		base.init();
		currentLevel = upgradeLevel;
	}

	// Update is called once per frame
	void Update()
    {
		tryShoot();
		checkUpgrades();
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

	protected override void tryShoot() //abstract?
	{
		target = furthestTarget();
		if (Time.time - lastShotTime > cooldown && target != null)
		{
			Vector3 dir = (target.transform.position - transform.position).normalized;

			GameObject bullet;
			if (target.transform.position.x >= transform.position.x)
            {
				bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
			}
            else
            {
				bullet = Instantiate(bulletTypes[1], transform.position, Quaternion.identity);
			}
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			AudioSource.PlayClipAtPoint(shoot, transform.position);
			Projectile projectile = bullet.GetComponent<Projectile>();
			bullet.GetComponent<Rigidbody2D>().velocity = dir * projectile.speed;
			projectile.target = target;
			projectile.damage = damage;
			Destroy(projectile.gameObject, bulletLifetime);
			lastShotTime = Time.time;
		}

	}


}
