using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsTower : Tower
{
    [SerializeField] private float asteroidSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShotTime > cooldown)
        {
            tryShoot();
            lastShotTime = Time.time;
        }
    }

    protected override void tryShoot()
    {
        /*
        float spawnX = 0;
        float spawnY = 0;
        int choice = Random.Range(0, 3);
        if (choice == 0) // spawn quadrant for asteroid: 0 = left, 1 = right, 2 = up, 3 = down
        {
            spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y);
            spawnX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.mainWindowPosition.x, 0)).x - 1;
        }
        else if (choice == 1)
        {
            spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y);
            spawnX = Camera.main.ScreenToWorldPoint(new Vector2(Screen.mainWindowPosition.x + Screen.width, 0)).x + 1;
        }
        else if (choice == 2)
        {
            spawnY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y)).y + 1;
            spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x + Screen.width)).x);
        }
        else if (choice == 3)
        {
            spawnY = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.y - Screen.height)).y - 1;
            spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.mainWindowPosition.x + Screen.width)).x);
        }*/


        GameObject asteroid = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - asteroid.transform.position).normalized;
        asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
        Destroy(asteroid,2);
    }
}
