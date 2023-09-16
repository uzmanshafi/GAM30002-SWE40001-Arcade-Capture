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

    private Tower selectedTower;
    
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
         moneyText.text = GameManager.instance.money.ToString();
         waveText.text = "Wave: "+ GameManager.instance.currentWave;
     }

     void Update()
     {
         updateTextUi();
     }

    // Initialises variables before the game starts
    void Awake()
    {
        instance = this;
    }
}