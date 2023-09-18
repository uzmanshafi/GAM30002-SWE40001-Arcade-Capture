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

    private List<Enemy> _allEnemies = new List<Enemy>();
    private List<Tower> _allTowers = new List<Tower>();

    public List<Enemy> AllEnemies => _allEnemies;
    public List<Tower> AllTowers => _allTowers;


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
            
            tower.cooldown = tower.base_cooldown;
            tower.range = tower.base_range;
            tower.damage = tower.base_damage; //tower doesn't have the damage on it, that must be on projectile somehow. Not sure how to make that work

            PowerPointTower ppt;
            //Get all the powerpoint towers using unity magic otherwise just do this
            foreach (var powerpoint_tower in AllTowers) {
                if (powerpoint_tower.TryGetComponent<PowerPointTower>(out ppt)) { //pseudocode

                    //if in range use powerpoint range
                    if (withinRange((Vector2)tower.transform.position, (Vector2)ppt.transform.position, ppt.range)) {
                        tower.cooldown *= ppt.cooldown_multipler;
                        tower.range *= ppt.range_multipler;
                        tower.damage *= ppt.damage_multiplier;
                    }

                } else {
                    continue;
                }
            }

        }
    }
}

