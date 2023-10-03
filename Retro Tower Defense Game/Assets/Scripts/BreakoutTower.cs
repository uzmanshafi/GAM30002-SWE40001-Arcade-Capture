using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutTower : PredictiveTower
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
    }
}
