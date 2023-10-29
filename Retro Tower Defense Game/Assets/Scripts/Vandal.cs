using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vandal : Enemy
{
    private Tower currentTower;
    private bool isVandalising;
    [SerializeField] private float vandaliseLength;
    [SerializeField] private float vandaliseCooldown;
    private float vandaliseTime; //Used for both how long vandal has been vandalising and cooldown

    void Start()
    {
        base.init();
        vandaliseTime = vandaliseCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        base.move();
        vandaliseTime += Time.deltaTime;
        if (!isVandalising && vandaliseTime >= vandaliseCooldown)
        {
            currentTower = GameManager.instance.AllTowers[Random.Range(0, GameManager.instance.AllTowers.Count - 1)];
            currentTower.enabled = false;
            isVandalising = true;
            vandaliseTime = 0f;
        }
        if (isVandalising)
        {
            if (vandaliseTime >= vandaliseLength)
            {
                currentTower.enabled = true;
                currentTower = null;
                isVandalising = false;
                vandaliseTime = 0f;
            }
        }
    }

    internal override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            GM.AllEnemies(true).Remove(this);
            GM.money += moneyOnKill;
            if (isVandalising)
            {
                currentTower.enabled = true;
                isVandalising = false;
            }
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
