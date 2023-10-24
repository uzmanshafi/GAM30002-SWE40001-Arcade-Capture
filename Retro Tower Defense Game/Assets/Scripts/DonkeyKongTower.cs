using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public class DonkeyKongTower : PredictiveTower
{
	private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        base.initialise();
		currentLevel = upgradeLevel;

		// Find closest point on path

		float nearest_point_distance_squared = float.MaxValue;
		Vector2? nearest_point = null;
		Waypoints waypoints = GameManager.instance.GetComponent<Waypoints>();

		// Loop through waypoints
		for (int i = 0; i < waypoints.Points.Length - 1; ++i) {
			// For each waypoint segment, get nearest point from tower position.
			Vector2 point = nearestPointOnSegment(this.transform.position, waypoints.Points[i], waypoints.Points[i+1]);
			// If distance is smaller than nearest_point_distance then update it as well as nearest_point
			if (((Vector2)this.transform.position - point).sqrMagnitude < nearest_point_distance_squared) {
				nearest_point = point;
				nearest_point_distance_squared = ((Vector2)this.transform.position - point).sqrMagnitude;
			}
		}

		//Set the targetting position to be nearest_point instead of using predictive__projectile

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
