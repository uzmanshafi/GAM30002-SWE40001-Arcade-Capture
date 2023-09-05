using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public TMP_Text moneyText;
    public TMP_Text starsText;
    public TMP_Text waveText;

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

    // public void updateTextUi()
    // {
    //     // starText.text = "Stars: " +  GameManager.instance.stars;
    //     moneyText.text = "Money: " +  GameManager.instance.money;
    //     waveText.text = "Wave: " +  GameManager.instance.currentWave;
    // }

    // private void Update()
    // {
    //     updateTextUi();
    // }

    // Initialises variables before the game starts
    private void Awake()
    {
        instance = this;
    }
}