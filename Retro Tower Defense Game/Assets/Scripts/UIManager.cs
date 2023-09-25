using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI waveText;

    public GameObject towerMenu;
    public GameObject upgradeMenu;

    private Tower? selectedTower;
    
    public void sellSelectedTower()
    {
        GameManager.instance.money += selectedTower.cost / 2;
        Destroy(selectedTower.gameObject);
        deselectTower();
    }
    
    public void selectTower(Tower t)
    {
        selectedTower = t;
        towerMenu.SetActive(false);
        upgradeMenu.SetActive(true);
    }

    public void deselectTower()
    {
        towerMenu.SetActive(true);
        upgradeMenu.SetActive(false);
    }

    //Should be working
     public void updateTextUi()
     {
         starText.text = "Stars: " +  GameManager.instance.stars;
        moneyText.text = formatNumber(GameManager.instance.money);
         waveText.text = "Wave: "+ GameManager.instance.currentWave;
     }

     void Update()
     {
        updateTextUi();

        if (Input.GetKeyDown(KeyCode.0)) {
            bool selectSomething = false;
            foreach (var Tower in GameManager.instance.allTowers) {
                if ((Tower.transform.position - MousePositionVector).sqrMagnitude <= Math.Pow(Tower.radius, 2)) {
                    selectedTower = Tower;
                    selectSomething = true;
                }
            }
            if (selectSomething == false) {
                selectedTower = null;
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