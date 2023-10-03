using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkeyKongTower : PredictiveTower
{
    // Start is called before the first frame update
    void Start()
    {
        base.initialise();
    }


    // Update is called once per frame
    void Update()
    {
        base.tryShoot();
        checkUpgrades();
    }

    private void checkUpgrades()
    {
        if (upgradeLevel == 1) 
        { 

        }
    }


}
