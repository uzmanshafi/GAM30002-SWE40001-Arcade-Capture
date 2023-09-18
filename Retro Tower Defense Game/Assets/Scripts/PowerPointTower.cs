using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPointTower : Tower
{
    [SerializeField] public float damage_multiplier = 1.0f;
    [SerializeField] public float cooldown_multipler = 1.0f; // 0.5 will shoot twice as fast
    [SerializeField] public float range_multipler = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void tryShoot() //abstract?
    {

    }

    
}
