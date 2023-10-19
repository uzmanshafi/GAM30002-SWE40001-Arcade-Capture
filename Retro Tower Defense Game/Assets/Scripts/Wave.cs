using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class Wave : MonoBehaviour
{
    [SerializeField] private string[] waveFileNames;
    [SerializeField] public GameObject[] enemyList;
    private SortedDictionary<float, WaveBatch> waveBatches = new SortedDictionary<float, WaveBatch>();
    private float timeSinceLastEnemy;
    private float waveTime;
    private int batchIndex = 0;
    private int waveIndex = 0;
    public bool waveInProgress = false;  // New flag to indicate if wave is in progress
    private GameManager gameManager;
    private Vector2 start;
    private bool waveLoaded = false;
    public bool waveEnded = true;

    private List<GameObject> spawnedEnemiesThisWave = new List<GameObject>();


    void Start()
    { //All just for testing

        gameManager = GameManager.instance;
        start = gameManager.GetComponent<Waypoints>().Points[0];
    }



    void Update()
    {
        if (waveInProgress)
        {
            waveTime += Time.deltaTime;

            if (waveBatches.Count > 0 && waveTime > waveBatches.First().Key)
            {
                waveEnded = false;

                if (Input.GetKeyDown(KeyCode.Alpha0)) //for testing purposes, delete before final build
                {
                    startWave();
                }

                KeyValuePair<float, WaveBatch> kvp = waveBatches.First();

                if (Time.time - timeSinceLastEnemy > kvp.Value.enemyCooldown)
                {
                    GameObject enemy = Instantiate(kvp.Value.Enemies[batchIndex], start, Quaternion.identity);
                    gameManager.AllEnemies(true).Add(enemy.GetComponent<Enemy>());
                    spawnedEnemiesThisWave.Add(enemy);
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

            if (waveBatches.Count <= 0 && AllEnemiesDestroyed())
            {
                waveLoaded = false;
                waveInProgress = false;
                gameManager.money += 30;
                waveEnded = true;
                spawnedEnemiesThisWave.Clear();
                if (gameManager.stars <= 9)
                {
                    StartCoroutine(IncreaseStarRating());
                }
            }
        }
        if (Input.GetKey(KeyCode.Space) && !gameObject.GetComponent<Wave>().waveInProgress)
        {
            startWave();
        }
    }
    private bool AllEnemiesDestroyed()
    {
        return spawnedEnemiesThisWave.All(e => e == null);
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
                        enemies2Spawn[i] = enemyList[int.Parse(enemyNums[i])];
                    }
                }
                else
                {
                    enemies2Spawn = new GameObject[] { enemyList[int.Parse(fields[1])] };
                }
                Debug.Log(fields[0] + '|' + enemies2Spawn + '|' + fields[2]);
                waveBatches.Add(time, new WaveBatch(enemies2Spawn, float.Parse(fields[2])));
                Debug.Log("Adding enemies at T = " + fields[0]);
            }
        }
    }

    private IEnumerator IncreaseStarRating()
    {
        yield return new WaitForSeconds(0.5f); // Waits for 1 second

        gameManager.stars += 1;
        gameManager.FlashStarRatingGreen();
    }


    public void startWave()
    {
        if (!waveLoaded && gameManager.AllEnemies(true).Count == 0 && waveIndex <= waveFileNames.Length - 1)
        {
            loadFromFile(Application.streamingAssetsPath + "/WaveFiles/" + waveFileNames[waveIndex]);
            waveTime = 0;
            waveLoaded = true;
            waveIndex++;
            waveInProgress = true;
            gameManager.currentWave += 1;
        }
    }
}
