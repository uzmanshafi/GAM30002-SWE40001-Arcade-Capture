using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AsteroidsTower : Tower
{
    [SerializeField] private float asteroidSpeed;
    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioMixerGroup soundGroup;

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
        SoundEffect.PlaySoundEffect(shoot, transform.position, 1, soundGroup);
        /*GameObject asteroid = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - asteroid.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
        asteroid.GetComponent<Projectile>().damage = damage;
        Destroy(asteroid, bulletLifetime);*/

        // This is the ratio that the first shot is from the origin
        float leftRatio = -((upgradeLevel + 1) % 2)/2.0f - ((upgradeLevel - 1) / 2.0f);
        
        for (int i = 0; i <= upgradeLevel; ++i) {

            Vector2 mouseWorldPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 perpendicular = new Vector2(transform.position.y - mouseWorldPosition.y, mouseWorldPosition.x - transform.position.x);
            
            Vector2 bulletOrigin = (Vector2)transform.position + perpendicular.normalized * (leftRatio + i) * spreadDistance;

            Vector2 dir = (mouseWorldPosition - bulletOrigin).normalized; //If they want a wall of shots instead of shots that all go toward the mouse then replace bulletOrigin with transform.position

            GameObject asteroid = Instantiate(bulletTypes[0], bulletOrigin, Quaternion.identity);

            // Pretty sure these lines are redundant
            // float angle = Mathf.Atan2(dir.y, dir.x);
            // dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Can we put this in the Instantiate function?
            asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
            Projectile astrProj = asteroid.GetComponent<Projectile>();
            astrProj.damage = damage;
            astrProj.color = controlColour;
            astrProj.canSeeCamo = canSeeCamo;
            astrProj.canSeeCamo2 = isStaffBuffed2;

            Destroy(asteroid, bulletLifetime); //What does this line do?

        }
    }

    
}
