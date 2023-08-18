using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class samplePooler : MonoBehaviour
{

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int WaveSize;
    [SerializeField] private int specialEnemies; //placeholder, could make new variable for each enemy type
    [SerializeField] private float EnemyDelay;

    private int numEnemies;
    private float timeSinceLast;
    private Vector2 start;

    private List<GameObject> enemyPool;
    private GameObject Enemies;

    private void Awake()
    {
        enemyPool = new List<GameObject>();
        Enemies = new GameObject("Enemy-Loader");
        CreateWave();
    }

    private void CreateWave()
    {
        for (int i = 0; i < WaveSize - specialEnemies; i++)
        {
            enemyPool.Add(CreateEnemy(enemies[0]));
        }
        for (int i = 0; i < specialEnemies; i++)
        {
            enemyPool.Add(CreateEnemy(enemies[1]));
        }
    }

    private GameObject CreateEnemy(GameObject enemyToCreate)
    {
        GameObject enemyInstance = Instantiate(enemyToCreate);
        enemyInstance.transform.SetParent(Enemies.transform);
        enemyInstance.SetActive(false);
        return enemyInstance;
    }

    public void removeMe(GameObject enemyToRemove) //either this or do not destory enemies and return them to the pool
    {
        if (enemyPool.Contains(enemyToRemove))
        {
            enemyPool.Remove(enemyToRemove);
        }
    }

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
        if (Time.time - timeSinceLast > EnemyDelay && numEnemies < WaveSize)
        {
            numEnemies++;
            spawnEnemyfromPool();
            timeSinceLast = Time.time;
        }
    }


    private void spawnEnemyfromPool()
    {
        for (int i = 0; i < WaveSize; i++)
        {
            if (!enemyPool[i].activeInHierarchy)
            {
                enemyPool[i].transform.position = start;
                enemyPool[i].SetActive(true);
                return;
            }
        }
        GameObject newEnemy = CreateEnemy(enemies[0]); //This should never happen
        newEnemy.transform.position = start;
        newEnemy.SetActive(true);
    }
}
