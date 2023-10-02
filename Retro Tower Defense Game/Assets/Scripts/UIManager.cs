using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI starText;
    public TextMeshProUGUI waveText;

    public GameObject towerMenu;
    public GameObject upgradeMenu;

    private GameObject currentRadius;
    public GameObject radiusPrefab;

    private float scaleFactor = 0.5f;
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

        //show radius here
        t.range = Instantiate(radiusPrefab, t.transform).GetComponent<CircleCollider2D>().radius;
        UpdateRadiusDisplay(t.range);
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

        bool selected = false;

         foreach (var tower in GameManager.instance.AllTowers)
         {
            Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (Math.Pow(tower.radius, 2) <= (mousepos - (Vector2)tower.transform.position).sqrMagnitude) {
                //Collision
                selected = true;
                selectTower(tower);
                break;
            } 
         }

         if (!selected) {
            deselectTower();
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

    private void UpdateRadiusDisplay(float range)
    {
        float desiredRadius = range * scaleFactor;
        currentRadius.transform.localScale = new Vector2(desiredRadius, desiredRadius);
    }

}