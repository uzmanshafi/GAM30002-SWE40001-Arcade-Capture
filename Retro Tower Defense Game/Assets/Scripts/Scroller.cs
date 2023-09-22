using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : Enemy
{

    [SerializeField] public string colour;

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
            GM.AllEnemies(true).Remove(this);
            GM.money += moneyOnKill;
            GameObject moneyonKillText = Instantiate(moneyText, transform.position, Quaternion.identity);
            Destroy(moneyonKillText, 0.3f);
            moneyonKillText.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "+" + moneyOnKill;

            //GameManager.instance.money += moneyOnKill;
            Destroy(gameObject);

        }
    }
}
