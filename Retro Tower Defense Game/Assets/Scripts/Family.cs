using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Family : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        base.init();
    }

    // Update is called once per frame
    void Update()
    {
        base.move();
    }

    public override void TakeDamage(float damage)
    {

    }
}
