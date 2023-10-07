using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIManager : MonoBehaviour
{
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
            if (GameManager.instance.AllTowers.Contains(pt))
            {
                GameManager.instance.AllTowers.Remove(pt);
            }
            Destroy(pt.other.gameObject);
        }
        GameManager.instance.money += (int)(selectedTower.cost * 1f);
        if (GameManager.instance.AllTowers.Contains(selectedTower))
        {
            GameManager.instance.AllTowers.Remove(selectedTower);
        }
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
                }
                if (g.name == "towerSprite")
                {
                    if (selectedTower.inspectSprite != null)
                    {
                        g.GetComponent<Image>().sprite = selectedTower.inspectSprite;
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
                GameManager.instance.money -= selectedTower.upgrades[selectedTower.upgradeLevel + 1].cost;
                selectedTower.upgradeLevel += 1;
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