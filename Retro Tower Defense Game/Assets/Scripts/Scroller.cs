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
        if (health <= 0 && !isDead)
        {
            isDead = true;
            GM.AllEnemies(true).Remove(this);
            GM.money += moneyOnKill;
            GameObject moneyonKillText = Instantiate(moneyText, transform.position, Quaternion.identity);
            Destroy(moneyonKillText, .9f);
            moneyonKillText.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "+" + moneyOnKill;
            SoundEffect.PlaySoundEffect(die, transform.position, 1, soundGroup);
            GameObject effect = Instantiate(coinplosion, transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity);
            effect.GetComponent<ParticleSystem>().Emit((int)(moneyOnKill * coinParticleMultiplier));
            //GameManager.instance.money += moneyOnKill;
            Destroy(gameObject);

        }
    }
}
