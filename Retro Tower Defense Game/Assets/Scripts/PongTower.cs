using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongTower : Tower
{
    private GameManager gameManager;

    private bool matched = false;

    public Projectile pongShot;
    public PongTower other;
    public Rigidbody2D shotRB;

    [SerializeField] private int pongOrder = 0; //Will assign this in tower placement, denotes which starts the pong shot
    [SerializeField] private GameObject pongPaddlePrefab;
    protected GameObject pongPaddle;

    [SerializeField] private AudioClip shoot;

    public int TowerOrder { get { return pongOrder; } set { pongOrder = value; } }

    // Start is called before the first frame update
    void Start()
    {
        base.init();
        gameManager = GameManager.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (upgradeLevel != other.upgradeLevel)
        {
            if (pongOrder == 0)
            {
                other.upgradeLevel = upgradeLevel;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            matchTowers();
        }
        if (other != null)
        {
            if (!matched)
            {
                face();
            }
            if (pongShot == null && matched && pongOrder == 0 && other.TowerOrder == 1)
            {
                tryShoot();
            }
        }
    }
    
    private void face()
    {
        transform.right = other.transform.position - transform.position;
        other.transform.right = transform.position - other.transform.position;
        if (pongOrder == 1)
        {
            pongPaddle = Instantiate(pongPaddlePrefab, transform);
            pongPaddle.GetComponent<PongPaddle>().parent = this;
            pongPaddle.transform.position = Vector3.MoveTowards(pongPaddle.transform.position, other.transform.position, .5f);
            pongPaddle.transform.eulerAngles = new Vector3(pongPaddle.transform.eulerAngles.x, pongPaddle.transform.eulerAngles.y, pongPaddle.transform.eulerAngles.z + 90);

            other.pongPaddle = Instantiate(pongPaddlePrefab, other.transform);
            other.pongPaddle.GetComponent<PongPaddle>().parent = other;
            other.pongPaddle.transform.position = Vector3.MoveTowards(other.pongPaddle.transform.position, transform.position, .5f);
            other.pongPaddle.transform.eulerAngles = new Vector3(other.pongPaddle.transform.eulerAngles.x, other.pongPaddle.transform.eulerAngles.y, other.pongPaddle.transform.eulerAngles.z + 90);
        }
        matched = true;
    }

    protected override void tryShoot()
    {
        Vector2 dir = other.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        AudioSource.PlayClipAtPoint(shoot, transform.position);
        pongShot = Instantiate(bulletTypes[0], gameObject.transform).GetComponent<Projectile>();
        pongShot.damage = damage;
        shotRB = pongShot.GetComponent<Rigidbody2D>();
        other.pongShot = pongShot;
        other.shotRB = shotRB;
        shotRB.velocity = dir * pongShot.speed;
    }

    public void sendBack()
    {
        shotRB.velocity = Vector2.zero;
        Vector2 dir = other.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        switch (upgradeLevel)
        {
            case 0:
                shotRB.velocity = dir * pongShot.speed;
                break;
            case 1:
                shotRB.velocity = dir * (pongShot.speed * 1.5f);
                break;
            case 2:
                shotRB.velocity = dir * (pongShot.speed * 2f);
                break;
            default:
                shotRB.velocity = dir * pongShot.speed;
                break;
        }
        AudioSource.PlayClipAtPoint(shoot, transform.position);
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
