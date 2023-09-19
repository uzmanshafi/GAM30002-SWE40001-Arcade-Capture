using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : Enemy
{

    [SerializeField] public string colour;

    void Start()
    {
        base.init();
    }

    // Update is called once per frame
    void Update()
    {
        base.move();
    }

    
}
