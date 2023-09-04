using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBatch
{

    private GameObject[] enemies;
    private float cooldown;


    public GameObject[] Enemies => enemies;
    public float enemyCooldown => cooldown;
    
    public WaveBatch(GameObject[] e, float cd )
    {
        enemies = e;
        cooldown = cd;
    }

}
