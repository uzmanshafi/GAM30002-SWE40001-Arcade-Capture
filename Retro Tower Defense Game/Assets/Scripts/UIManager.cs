using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public TMP_Text moneyText;
    // public TMP_Text starsText;
    // public TMP_Text waveText;

    public GameObject towerMenu;
    public GameObject upgradeMenu;
    private Tower selectedTower;
    // private void Update()
    // {
    //     updateTextUi();
    // }

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
    private void Awake()
    {
        instance = this;
    }
    
    // public void updateTextUi()
    // {
    //     livesText.text = "Lives: " +  GameManager.instance.lives;
    //     moneyText.text = "Cash: " +  GameManager.instance.money;
    //     // waveText.text = "Wave: " +  GameManager.instance.currentWave;
    // }
}
