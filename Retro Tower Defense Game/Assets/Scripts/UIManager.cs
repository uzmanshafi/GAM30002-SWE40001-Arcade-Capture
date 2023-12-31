using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] protected AudioClip sell;
    [SerializeField] protected AudioClip upgrade;
    [SerializeField] protected AudioMixerGroup soundGroup;
    [SerializeField] protected GameObject moneyTextEffect;
    [SerializeField] protected GameObject upgradeEffect;
    [SerializeField] protected GameObject coinplosion;

    public static UIManager instance;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI waveText;

    public GameObject towerMenu;
    public GameObject upgradeMenu;

    public Slider starBar;

    private Tower selectedTower;


    public void sellSelectedTower()
    {
        if (selectedTower.TryGetComponent<PongTower>(out PongTower pt))
        {
            if (GameManager.instance.AllTowers.Contains(pt.other))
            {
                GameManager.instance.AllTowers.Remove(pt.other);
            }
            Destroy(pt.other.gameObject);
        }
        GameManager.instance.money += (int)(selectedTower.cost * 1f);
        if (GameManager.instance.AllTowers.Contains(selectedTower))
        {
            GameManager.instance.AllTowers.Remove(selectedTower);
        }
        GameObject moneyonKillText = Instantiate(moneyTextEffect, selectedTower.transform.position, Quaternion.identity);
        Destroy(moneyonKillText, .9f);
        moneyonKillText.GetComponentInChildren<TextMeshProUGUI>().text = "+" + (int)(selectedTower.cost * 1f);
        SoundEffect.PlaySoundEffect(sell, selectedTower.gameObject.transform.position, 1, soundGroup);
        GameObject effect = Instantiate(coinplosion, selectedTower.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity);
        effect.GetComponent<ParticleSystem>().Emit((int)(selectedTower.cost * 1f));
        Destroy(selectedTower.gameObject);
        deselectTower();
    }

    public void selectTower(Tower t)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        selectedTower = t;
        upgradeMenu.SetActive(true);
    }

    public void deselectTower()
    {
        upgradeMenu.SetActive(false);

        TowerPlacement towerPlacement = FindObjectOfType<TowerPlacement>();
        if (towerPlacement != null)
        {
            towerPlacement.DestroyCurrentRadius();

            // Additional code to deactivate LineRadiusPrefab for the SpaceInvaders tower
            if (selectedTower != null && selectedTower.gameObject.name.StartsWith("SpaceInvaders"))
            {
                towerPlacement.DeactivateSpaceInvaderRadius(selectedTower.gameObject);
            }
        }
        selectedTower = null;  // Ensure to set the selectedTower to null after deselecting
    }

    //Should be working
    public void updateTextUi()
    {
        //starText.text = "Stars: " +  GameManager.instance.stars;
        moneyText.text = formatNumber(GameManager.instance.money);
        waveText.text = "Wave: " + GameManager.instance.currentWave;
        if (selectedTower)
        {
            if (selectedTower.TryGetComponent<PongTower>(out PongTower pt) && pt.TowerOrder == 1)
            {
                selectedTower = pt.other;
            }
            foreach (Transform g in upgradeMenu.transform)
            {
                if (g.name == "towerName")
                {
                    if (g.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI tmp))
                    {
                        string towerName = selectedTower.name;
                        string[] towerNameSplit = towerName.Split("("); // Used to remove (Clone) appearing after name

                        tmp.text = towerNameSplit[0];
                    }
                }
                if (g.name == "currentUpgradeLevel")
                {
                    if (g.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI tmp))
                    {
                        tmp.text = "Level: " + (selectedTower.upgradeLevel + 1);
                    }
                }
                if (g.name == "sellTower")
                {
                    TextMeshProUGUI tmp = g.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = "Sell $" + selectedTower.cost * 1f;
                    }
                }
                if (g.name == "buyUpgrade")
                {
                    TextMeshProUGUI tmp = g.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmp != null)
                    
                        if (tmp.name == "CostText")
                        {
                            //Debug.Log(selectedTower.upgradeLevel + " " + selectedTower.upgrades.Length);
                            if (selectedTower.upgradeLevel < 2)
                            {
                                tmp.text = "UPGRADE $" + selectedTower.upgrades[selectedTower.upgradeLevel + 1].cost;
                            }
                            else
                            {
                                tmp.text = "MAXED";
                            }
                        }
                        else
                        {
                            if (selectedTower.upgradeLevel < 1)
                            {
                                tmp.text = selectedTower.towerUpgrade;
                            }
                            else
                            {
                                tmp.text = selectedTower.towerUpgrade2;
                            }
                        }

                    
                }
                if (g.name == "towerSprite")
                {
                    if (selectedTower.inspectSprite != null)
                    {
                        g.GetComponent<Image>().sprite = selectedTower.inspectSprite;
                    }
                }
                if (g.name == "PowerPointBuff")
                {
                    if (selectedTower.isPowerPointBuffed)
                    {
                        g.gameObject.SetActive(true);
                    }
                    else
                    {
                        g.gameObject.SetActive(false);
                    }
                }
                if (g.name == "StaffBuff")
                {
                    if (selectedTower.isStaffBuffed || selectedTower.isStaffBuffed2)
                    {
                        g.gameObject.SetActive(true);
                    }
                    else
                    {
                        g.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    void Update()
    {
        updateTextUi();
        updateStars();
    }

    private void updateStars()
    {
        starBar.value = GameManager.instance.stars;
    }

    public void upgradeSelectedTower()
    {
        if (selectedTower.upgradeLevel < 2)
        {
            if (GameManager.instance.money - selectedTower.upgrades[selectedTower.upgradeLevel + 1].cost >= 0)
            {
                SoundEffect.PlaySoundEffect(upgrade, selectedTower.transform.position, 1, soundGroup);
                GameManager.instance.money -= selectedTower.upgrades[selectedTower.upgradeLevel + 1].cost;
                GameObject moneyonUpgradeText = Instantiate(moneyTextEffect, selectedTower.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                Destroy(moneyonUpgradeText, .9f);
                moneyonUpgradeText.GetComponentInChildren<TextMeshProUGUI>().text = "-" + selectedTower.upgrades[selectedTower.upgradeLevel + 1].cost;
                moneyonUpgradeText.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                GameObject upgradeText = Instantiate(upgradeEffect, selectedTower.transform.position + new Vector3(0,0,0), Quaternion.identity);
                Destroy(upgradeText, .9f);
                selectedTower.upgradeLevel += 1;
                selectedTower.GetComponent<SpriteRenderer>().sprite = selectedTower.upgradeSprites[selectedTower.upgradeLevel];
            }
        }
    }

    // Initialises variables before the game starts
    void Awake()
    {
        instance = this;
    }

    private string formatNumber(int num)
    {
        if (num >= 100000000)
            return (num / 1000000).ToString("#,0M");

        if (num >= 10000000)
            return (num / 1000000).ToString("0.#") + "M";

        if (num >= 100000)
            return (num / 1000).ToString("#,0K");

        if (num >= 10000)
            return (num / 1000).ToString("0.#") + "K";

        return num.ToString("#,0");
    }


}