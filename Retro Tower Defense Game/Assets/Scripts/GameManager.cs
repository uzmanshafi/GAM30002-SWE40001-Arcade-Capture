using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TowerButton ClickedButton {get; private set;}
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickTower(TowerButton TowerButton)
    {
        this.ClickedButton = TowerButton;
    }
}
