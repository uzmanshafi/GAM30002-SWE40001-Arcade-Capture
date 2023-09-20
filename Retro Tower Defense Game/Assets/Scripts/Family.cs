using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Family : Enemy
{
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
        if (health <= 0)
        {
            GM.AllEnemies.Remove(this);
            GM.money += moneyOnKill;
            //GameManager.instance.money += moneyOnKill;

            Destroy(gameObject);

        }
    }
}
