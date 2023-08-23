using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Wave : MonoBehaviour
{
    private SortedDictionary<float, WaveBatch> waveBatches = new SortedDictionary<float, WaveBatch>();
    
    private float timeSinceLastEnemy;
    private int batchIndex = 0;

    private Vector2 start;



    void Start()
    { //All just for testing
        start = gameObject.GetComponent<Waypoints>().Points[0];
        GameObject[] enemies = GameObject.FindAnyObjectByType<samplePooler>().enemies;
        GameObject[] Pink3 = { enemies[0], enemies[0], enemies[0]};
        WaveBatch batch1 = new WaveBatch(Pink3, 0.8f);
        waveBatches.Add(2f, batch1);
        GameObject[] wave2E = { enemies[0], enemies[0], enemies[1], enemies[1] };
        WaveBatch batch2 = new WaveBatch(wave2E, 1.5f);
        for (int i = 0; i < 10; i++)
        {
            waveBatches.Add(i * 6f, batch2);
        }
        for (int i = 0; i < 5; i++)
        {
            //waveBatches.Add(i * 8f, batch1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (waveBatches.Count > 0 && Time.time > waveBatches.First().Key)
        {
            KeyValuePair<float, WaveBatch> kvp = waveBatches.First();
            if (Time.time - timeSinceLastEnemy > kvp.Value.enemyCooldown)
            {
                Instantiate(kvp.Value.Enemies[batchIndex], start, Quaternion.identity);
                batchIndex++;
                timeSinceLastEnemy = Time.time;
            }
            if (batchIndex >= kvp.Value.Enemies.Length)
            {
                waveBatches.Remove(kvp.Key);
                batchIndex = 0;
            }
        }
        else
        {
            timeSinceLastEnemy = 0f;
        }
    }
}
