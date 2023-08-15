using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class samplePooler : MonoBehaviour
{

    [SerializeField] private GameObject enemy;
    [SerializeField] private int EnemyCount;
    [SerializeField] private float EnemyDelay;

    private int numEnemies;
    private float timeSinceLast;
    private Vector2 start;

    // Start is called before the first frame update
    void Start()
    {
        numEnemies = 0;
        start = gameObject.GetComponent<Waypoints>().Points[0];
        timeSinceLast = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeSinceLast > EnemyDelay && numEnemies < EnemyCount)
        {
            numEnemies++;
            Instantiate(enemy, start, Quaternion.identity);
            timeSinceLast = Time.time;
        }
    }
}
