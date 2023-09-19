using System;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    [SerializeField] public float cost;
    [SerializeField] protected float cooldown = 1;

    protected float actual_cooldown = cooldown; //This gets modified externally so the base cooldown is not changed

    protected float lastShotTime; //used to determine cooldown
    [SerializeField] protected Enemy target;

    [SerializeField] public float towerRadius;
    [SerializeField] public float range = 5;

    protected float actual_range = range; //This gets modified externally so the base range is not changed
    [SerializeField] public float radius;
    [SerializeField] public GameObject radiusDisplay;
    [SerializeField] public GameObject mesh;
    [SerializeField] protected GameObject[] bulletTypes;

    protected bool canSeeCamo;

    // public int upgrade

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tryShoot();
    }
     
    public void selected(bool t)
    {
        radiusDisplay.SetActive(t);
        radiusDisplay.transform.localScale = new Vector2(towerRadius * 2, towerRadius * 2);
    }

    protected abstract void tryShoot();

    protected Enemy? furthestTarget()
    {

        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, actual_range, Vector2.up, LayerMask.GetMask("Enemy")); //Raycast and return any objects on layer enemy: Check if raycast is efficient
        if (results.Length == 0)
        {
            target = null;
        }

        Enemy[] enemies_in_range = results.Select(e => e.collider.gameObject.TryGetComponent<Enemy>()).ToArray();

        float bestDistance = Mathf.Infinity;
        Enemy bestEnemy = null;
        float tempDistance;
        Vector3 endPoint;
        if (enemies_in_range.Length > 0)
        {
            foreach (Enemy e in enemies_in_range) // find enemy closest to end point (Distance not furthest on path)
            {
                if (e == null) { continue; }
                if (e.IsCamo && !canSeeCamo) { continue; }
                endPoint = e.GetWaypoints.Points[e.GetWaypoints.Points.Length - 1];
                tempDistance = Vector2.Distance(e.transform.position, endPoint);
                if (tempDistance < bestDistance)
                {
                    bestDistance = tempDistance;
                    bestEnemy = e;
                }
            }
            return bestEnemy;
        }
        return null;
    }
}
