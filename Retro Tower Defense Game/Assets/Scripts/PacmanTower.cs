using UnityEngine;

public class PacmanTower : Tower
{
    [Header("Level 1 Configuration")]
    [SerializeField] private float level1GhostSpeed = 1.0f;
    [SerializeField] private float level1GhostDamage = 1.0f;

    [Header("Level 2 Configuration")]
    [SerializeField] private float level2GhostSpeed = 2.0f;
    [SerializeField] private float level2GhostDamage = 2.0f;

    [Header("Temp Levels Configuration")]
    public bool isLevel1 = true;
    public bool isLevel2 = false;

    public bool isLevel3 = false;

    private GameManager gameManager;
    private Waypoints waypoints;
    private Vector3 exitPoint;

    void OnValidate()
    {
        if (isLevel1)
        {
            isLevel2 = false;
            isLevel3 = false;
        }
        else if (isLevel2)
        {
            isLevel1 = false;
            isLevel3 = false;
        }
        else if (isLevel3)
        {
            isLevel1 = false;
            isLevel2 = false;
        }
    }

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        waypoints = gameManager.GetComponent<Waypoints>();
        exitPoint = waypoints.Points[waypoints.Points.Length - 1];

        base.init();

        if (isLevel1)
        {
            ConfigureLevel1();
        }
        else if (isLevel2)
        {
            ConfigureLevel2();
        }
    }

    void Update()
    {
        tryShoot();
    }

    void ConfigureLevel1()
    {
        cooldown = 6.0f;
    }

    void ConfigureLevel2()
    {
        cooldown = 3.0f;
    }

    protected override void tryShoot()
    {
        if (Time.time - lastShotTime > cooldown && GameManager.instance.AllEnemies(true).Count != 0)
        {
            if (isLevel1)
            {
                SpawnGhost(level1GhostSpeed, level1GhostDamage);
            }
            else if (isLevel2)
            {
                SpawnGhost(level2GhostSpeed, level2GhostDamage);
            }
            lastShotTime = Time.time;
        }
    }

    void SpawnGhost(float speed, float damage)
    {
        GameObject ghostPrefab = bulletTypes[Random.Range(0, bulletTypes.Length)];
        GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);

        GhostProjectiles ghostScript = ghost.GetComponent<GhostProjectiles>();
        ghostScript.SetExitPoint(exitPoint);
        ghostScript.SetWaypoints(waypoints.Points);
        ghostScript.SetStats(speed, damage);
        ghost.SetActive(false);
        gameManager.AddGhost(ghost);
    }
}
