using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongTower : Tower
{
    private GameManager gameManager;

    public Projectile pongShot;
    public PongTower other;
    public Rigidbody2D shotRB;

    [SerializeField] private int pongOrder = 0; //Will assign this in tower placement, denotes which starts the pong shot
    

    // Start is called before the first frame update
    void Start()
    {
        base.init();
        gameManager = GameManager.instance;
        matchTowers();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            matchTowers();
        }
        if (other != null)
        {
            if (pongShot == null && pongOrder == 0)
            {
                tryShoot();
            }
        }
    }

    protected override void tryShoot()
    {
        Vector2 dir = other.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        pongShot = Instantiate(bulletTypes[0], gameObject.transform).GetComponent<Projectile>();
        shotRB = pongShot.GetComponent<Rigidbody2D>();
        other.pongShot = pongShot;
        other.shotRB = shotRB;
        shotRB.velocity = dir * pongShot.speed;
    }

    private void sendBack()
    {
        shotRB.velocity = Vector2.zero;
        Vector2 dir = other.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        shotRB.velocity = dir * pongShot.speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == pongShot.gameObject)
        {
            sendBack();
        }
    }

    private void matchTowers()
    {
        float bestDistance = 0;
        float tempDistance;
        Tower bestTower = null;
        foreach (Tower t in gameManager.AllTowers)
        {
            if (!t.TryGetComponent<PongTower>(out PongTower pt) || pt == this)
            {
                continue;
            }
            if (bestTower == null)
            {
                bestDistance = Vector2.Distance(transform.position, t.transform.position);
                if (bestDistance < range) //will only match if it is in range to do so
                {
                    bestTower = t;
                }
                
            }
            else
            {
                tempDistance = Vector2.Distance(transform.position, t.transform.position);
                if (tempDistance < bestDistance)
                {
                    bestDistance = tempDistance;
                    if (bestDistance < range) //will only match if it is in range to do so
                    {
                        bestTower = t;
                    }
                }
            }
        }
        if (bestTower != null)
        {
            other = bestTower.GetComponent<PongTower>();
            other.other = this;
            other.pongOrder = 1;
            transform.right = other.transform.position - transform.position;
            other.transform.right = transform.position - other.transform.position;
        }
    }
}
