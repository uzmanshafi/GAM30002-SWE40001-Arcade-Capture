using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wave : MonoBehaviour
{
    private SortedDictionary<WaveBatch, float> waveBatches = new SortedDictionary<WaveBatch, float>();
    
    private float timeSinceLastEnemy;
    private int batchIndex = 0;

    private Vector2 start;



    void Start()
    { //All just for testing
        start = gameObject.GetComponent<Waypoints>().Points[0];
        GameObject[] enemies = GameObject.FindAnyObjectByType<samplePooler>().enemies;
        GameObject[] wave1E = { enemies[0], enemies[0], enemies[1], enemies[1], enemies[0], enemies[1], enemies[0], enemies[1] };
        WaveBatch batch1 = new WaveBatch(wave1E, 0.3f);
        waveBatches.Add(batch1, 5f);
        GameObject[] wave2E = { enemies[0], enemies[0], enemies[1], enemies[1] };
        WaveBatch batch2 = new WaveBatch(wave2E, 0.5f);
        for (int i = 0; i < 10; i++)
        {
            waveBatches.Add(batch2, i * 3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >  waveBatches.First().Value)
        {
            KeyValuePair<WaveBatch, float> kvp = waveBatches.First();
            if (Time.time - timeSinceLastEnemy > kvp.Key.enemyCooldown)
            {
                Instantiate(kvp.Key.Enemies[batchIndex], start, Quaternion.identity);
                batchIndex++;
                timeSinceLastEnemy = Time.time;
            }
            if (batchIndex >= kvp.Key.Enemies.Length)
            {
                waveBatches.Remove(kvp.Key);
            }
        }
        else
        {
            timeSinceLastEnemy = 0f;
        }
    }
}
