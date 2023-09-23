using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AsteroidsTower : Tower
{
    [SerializeField] private float asteroidSpeed;

    [SerializeField] private float spreadDistance;
    // Start is called before the first frame update
    void Start()
    {
        base.init();
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
        // This is the ratio that the first shot is from the origin
        float leftRatio = -((upgradeLevel + 1) % 2)/2.0f - (int)((upgradeLevel - 1) / 2);

        for (int i = 0; i < upgradeLevel; ++i) {

            Vector2 bulletOrigin = (Vector2)transform.position + Vector2.Perpendicular(Input.mousePosition - transform.position) * (leftRatio + i) * spreadDistance;

            Vector2 dir = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - bulletOrigin; //If they want a wall of shots instead of shots that all go toward the mouse then replace bulletOrigin with transform.position

            GameObject asteroid = Instantiate(bulletTypes[0], bulletOrigin, Quaternion.identity);

            // Pretty sure these lines are redundant
            // float angle = Mathf.Atan2(dir.y, dir.x);
            // dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            // Can we put this in the Instantiate function?
            asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;


            Destroy(asteroid, 2); //What does this line do?

        }
    }
}
