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
            fire();
            lastShotTime = Time.time;
        }
    }

    private void fire()
    {
        float spawnY = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height + 10)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width + 10, 0)).x);
        GameObject asteroid = Instantiate(bulletTypes[2], new Vector2(spawnX, spawnY), Quaternion.identity);
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - asteroid.transform.position).normalized;
        asteroid.GetComponent<Rigidbody2D>().velocity = dir * asteroidSpeed;
        Destroy(asteroid,5);
    }
}
