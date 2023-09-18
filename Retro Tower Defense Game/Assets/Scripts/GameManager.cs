using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Utils;

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

    // Update is called once per frame. It doesn't matter what order this is called as after the first cycle the values will remain the same most of the time, unless a tower is moved/sold/upgraded. That does mean that we could actually only call this on those events, but that can happen another day
    void Update()
    {
       applyBuffs();
    }

    void applyBuffs() {
        foreach (var tower in AllTowers) {
            
            tower.actual_cooldown = tower.cooldown;
            tower.actual_range = tower.range;
            tower.actual_damage = tower.damage; //tower doesn't have the damage on it, that must be on projectile somehow. Not sure how to make that work

            //Get all the powerpoint towers using unity magic otherwise just do this
            foreach (var powerpoint_tower in AllTowers) {
                if (powerpoint_tower is type of PowerPointTower) { //pseudocode

                    //if in range use powerpoint range
                    if (withinRange((Vector2)tower.position, (Vector2)powerpoint_tower.position, powerpoint_tower.range)) {
                        tower.actual_cooldown *= powerpoint_tower.cooldown_multipler;
                        tower.actual_range *= powerpoint_tower.range_multipler;
                        tower.actual_damage *= powerpoint_tower.damage_multiplier;
                    }

                } else {
                    continue;
                }
            }

        }
    }
}

