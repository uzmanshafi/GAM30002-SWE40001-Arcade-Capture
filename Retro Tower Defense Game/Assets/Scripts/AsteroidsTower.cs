using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsTower : Tower
{
    [SerializeField] private float asteroidSpeed;
    [SerializeField] private AudioClip shoot;

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
        AudioSource.PlayClipAtPoint(shoot, transform.position);
        /*GameObject asteroid = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - asteroid.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
        asteroid.GetComponent<Projectile>().damage = damage;
        Destroy(asteroid, bulletLifetime);*/

        // This is the ratio that the first shot is from the origin
        float leftRatio = -((upgradeLevel + 1) % 2)/2.0f - ((upgradeLevel - 1) / 2.0f);
        Debug.Log(leftRatio);
        for (int i = 0; i <= upgradeLevel; ++i) {

            Vector2 perpendicular = new Vector2(transform.position.y - Input.mousePosition.y, Input.mousePosition.x - transform.position.x);
            Vector2 bulletOrigin = (Vector2)transform.position + perpendicular.normalized * (leftRatio + i) * spreadDistance;


            Vector2 dir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - bulletOrigin).normalized; //If they want a wall of shots instead of shots that all go toward the mouse then replace bulletOrigin with transform.position

            GameObject asteroid = Instantiate(bulletTypes[0], bulletOrigin, Quaternion.identity);

            // Pretty sure these lines are redundant
            // float angle = Mathf.Atan2(dir.y, dir.x);
            // dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Can we put this in the Instantiate function?
            asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
            Debug.Log(bulletOrigin);

            Destroy(asteroid, bulletLifetime); //What does this line do?

        }
    }

    
}
