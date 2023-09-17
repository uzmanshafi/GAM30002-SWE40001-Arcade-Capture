using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float stars = 5;
    public int money = 500;
    public int currentWave = 0;

    private List<GameObject> _allEnemies = new List<GameObject>();
    private List<GameObject> _allTowers = new List<GameObject>();

    public List<GameObject> AllEnemies => _allEnemies;
    public List<GameObject> AllTowers => _allTowers;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Tower"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Tower"), LayerMask.NameToLayer("DonkeyKongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("DonkeyKongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("PongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pathing"), LayerMask.NameToLayer("Projectile")); 
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pathing"), LayerMask.NameToLayer("PongProjectile"));
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public bool pointReached(Vector2 position, Vector2 destination, float threshold = 0.01f)
    {
        float distanceToNext = (position - destination).sqrMagnitude;
        return distanceToNext <= threshold;
    }

}

