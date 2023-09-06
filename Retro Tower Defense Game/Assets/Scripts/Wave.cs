using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class Wave : MonoBehaviour
{
    [SerializeField] private string[] waveFileNames;

    private SortedDictionary<float, WaveBatch> waveBatches = new SortedDictionary<float, WaveBatch>();

    private float timeSinceLastEnemy;
    private float waveTime;
    private int batchIndex = 0;
    private int waveIndex = 0;

    private GameObject[] enemies;

    private GameManager gameManager;

    private Vector2 start;

    private bool waveLoaded = false;

    void Start()
    { //All just for testing
        start = gameObject.GetComponent<Waypoints>().Points[0];
        enemies = GameObject.FindAnyObjectByType<samplePooler>().enemies;
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        waveTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha0) && !waveLoaded && waveIndex <= waveFileNames.Length - 1)
        {
            loadFromFile("WaveFiles/" + waveFileNames[waveIndex]);
            waveTime = 0;
            waveLoaded = true;
            waveIndex++;
            gameManager.currentWave += 1;
        }
        if (waveBatches.Count > 0 && waveTime > waveBatches.First().Key)
        {
            KeyValuePair<float, WaveBatch> kvp = waveBatches.First();
            if (Time.time - timeSinceLastEnemy > kvp.Value.enemyCooldown)
            {
                GameObject enemy = Instantiate(kvp.Value.Enemies[batchIndex], start, Quaternion.identity);
                gameManager.AllEnemies.Add(enemy);
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
        if (waveBatches.Count <= 0)
        {
            waveLoaded = false;
        }
    }

    public void loadFromFile(string fileName)
    {
        Debug.Log("starting load");
        waveBatches = new SortedDictionary<float, WaveBatch>();
        using (var parser = new StreamReader(fileName))
        {
            while (!parser.EndOfStream)
            {

                string[] fields = parser.ReadLine().Split(',');
                foreach (string s in fields)
                {
                    s.Trim();
                }
                if (fields[0] == "Time") { continue; }
                float time;
                if (!float.TryParse(fields[0], out time))
                {
                    continue;
                }
                GameObject[] enemies2Spawn;
                int numEnemies = 1;
                if (fields[1].Contains("|"))
                {
                    numEnemies = fields[1].Count(f => (f == '|')) + 1;
                    enemies2Spawn = new GameObject[numEnemies];
                    string[] enemyNums = fields[1].Split('|');
                    for (int i = 0; i < numEnemies; i++)
                    {
                        enemies2Spawn[i] = enemies[int.Parse(enemyNums[i])];
                    }
                }
                else
                {
                    enemies2Spawn = new GameObject[] { enemies[int.Parse(fields[1])] };
                }
                Debug.Log(fields[0] + '|' + enemies2Spawn + '|' + fields[2]);
                waveBatches.Add(time, new WaveBatch(enemies2Spawn, float.Parse(fields[2])));
                Debug.Log("Adding enemies at T = " + fields[0]);
            }
        }
    }


}
