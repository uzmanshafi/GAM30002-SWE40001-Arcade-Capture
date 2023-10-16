using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Family : Enemy
{
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        base.init();
    }

    // Update is called once per frame
    void Update()
    {
        base.move();
    }

    internal override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            GM.AllEnemies(true).Remove(this);
            GM.money += moneyOnKill;
            //GameManager.instance.money += moneyOnKill;
            GameObject[] enemyList = GM.GetComponent<Wave>().enemyList;
            Enemy[] spawnedEnemies = new Enemy[4];
            spawnedEnemies[0] = Instantiate(enemyList[0], transform.position, Quaternion.identity).GetComponent<Enemy>();
            spawnedEnemies[1] = Instantiate(enemyList[1], transform.position, Quaternion.identity).GetComponent<Enemy>();
            spawnedEnemies[2] = Instantiate(enemyList[2], transform.position, Quaternion.identity).GetComponent<Enemy>();
            spawnedEnemies[3] = Instantiate(enemyList[2], transform.position, Quaternion.identity).GetComponent<Enemy>();
            foreach(Enemy e in spawnedEnemies)
            {
                e.SetWaypoints = GetWaypoints;
                e.setWaypointIndex = getWaypointIndex;
                e.SetDestination = GetDestination;
                e.spawnAtStart = false;
            }
            GameObject moneyonKillText = Instantiate(moneyText, transform.position, Quaternion.identity);
            Destroy(moneyonKillText, .9f);
            moneyonKillText.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "+" + moneyOnKill;
            Destroy(gameObject);

        }
    }
}
