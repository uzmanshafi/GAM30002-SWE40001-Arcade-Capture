using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkeyKongTower : PredictiveTower
{
	private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        base.initialise();
		currentLevel = upgradeLevel;
	}


    // Update is called once per frame
    void Update()
    {
        base.tryShoot();
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


}
