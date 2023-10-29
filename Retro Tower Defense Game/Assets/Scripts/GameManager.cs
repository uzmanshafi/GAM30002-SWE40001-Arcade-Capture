using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using static Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float stars = 10;
    public int money = 500;
    public int currentWave = 0;

    [SerializeField] private AudioSource ghostAmbient;
    [SerializeField] private AudioSource wakaWaka;
    [SerializeField] protected AudioClip loseLife;
    [SerializeField] protected AudioClip gainLife;
    [SerializeField] protected AudioMixerGroup soundGroup;

    private List<Enemy> _allEnemies = new List<Enemy>();
    private List<Tower> _allTowers = new List<Tower>();
    private List<GameObject> _allGhosts = new List<GameObject>();

    private float timeSinceLastGhost = 0;

    public GameObject PAXGameOver;
    public GameObject PAXGameWon;
    public GameObject tooltip;
    public TextMeshProUGUI toolText;



[SerializeField] private Image starRatingFill;

    private Color originalStarColor;





    public List<Enemy> AllEnemies(bool canSeeCamo)
    {
        if (canSeeCamo == true)
        {
            return _allEnemies;
        }
        else
        {
            return _allEnemies.Where(e => !e.IsCamo).ToList();
        }
    }
    public List<Tower> AllTowers => _allTowers;

    public void AddGhost(GameObject ghost)
    {
        _allGhosts.Add(ghost);
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Tower"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Tower"), LayerMask.NameToLayer("DonkeyKongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("DonkeyKongProjectile"), LayerMask.NameToLayer("DonkeyKongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("DonkeyKongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("PongProjectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pathing"), LayerMask.NameToLayer("Projectile"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pathing"), LayerMask.NameToLayer("PongProjectile"));

        originalStarColor = starRatingFill.color;
    }

    // Update is called once per frame. It doesn't matter what order this is called as after the first cycle the values will remain the same most of the time, unless a tower is moved/sold/upgraded. That does mean that we could actually only call this on those events, but that can happen another day
    void Update()
    {
        applyBuffs();
        timeSinceLastGhost -= Time.deltaTime;
        if (_allGhosts.Count > 0 && timeSinceLastGhost <= 0)
        {
            _allGhosts[0].SetActive(true);
            _allGhosts.RemoveAt(0);
            timeSinceLastGhost = 0.15f;
        }
        GhostProjectiles[] ghosts = FindObjectsOfType(typeof(GhostProjectiles)) as GhostProjectiles[];
        if (ghosts.Length > 0)
        {
            ghostAmbient.mute = false;
        }
        else
        {
            ghostAmbient.mute = true;
        }

        if (stars <= 0)
        {
            PAXGameOver.SetActive(true);
        }

        //tooltip text check/changes
        //TooltipDisplay();

        if (currentWave == 30 && !gameObject.GetComponent<Wave>().waveInProgress)
        {
            PAXGameWon.SetActive(true);
        }

        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.P))
        //{
        //    SceneManager.LoadScene("PAXSplash");
        //}

    }

    public void RemoveTower(Tower tower)
    {
        if (_allTowers.Contains(tower))
        {
            _allTowers.Remove(tower);
        }
    }
    void applyBuffs()
    {
        foreach (var tower in AllTowers)
        {

            tower.cooldown = tower.base_cooldown;
            tower.range = tower.base_range;
            tower.damage = tower.base_damage; //tower doesn't have the damage on it, that must be on projectile somehow. Not sure how to make that work
            tower.isPowerPointBuffed = false;
            tower.isStaffBuffed = false;
            tower.isStaffBuffed2 = false;

            PowerPointTower ppt;
            //Get all the powerpoint towers using unity magic otherwise just do this
            foreach (var powerpoint_tower in AllTowers)
            {
                if (powerpoint_tower.TryGetComponent<PowerPointTower>(out ppt))
                { //pseudocode

                    //if in range use powerpoint range
                    if (withinRange((Vector2)tower.transform.position, (Vector2)ppt.transform.position, ppt.range))
                    {
                        tower.cooldown *= ppt.cooldown_multipler;
                        tower.range *= ppt.range_multipler;
                        tower.damage *= ppt.damage_multiplier;
                        tower.isPowerPointBuffed = true;
                    }

                }
                else if (powerpoint_tower.TryGetComponent<StaffTower>(out StaffTower st))
                {
                    if (withinRange((Vector2)tower.transform.position, (Vector2)st.transform.position, st.range))
                    {
                        if(st.upgradeLevel == 0)
                        {
                            tower.isStaffBuffed = true;
                        }
                        else
                        {
                            tower.isStaffBuffed2 = true;
                        }

                    }
                }
                else
                {
                    continue;
                }
            }

        }
    }

    public void FlashStarRatingRed()
    {
        SoundEffect.PlaySoundEffect(loseLife, transform.position, 1, soundGroup);
        StartCoroutine(FlashColorCoroutine(Color.red));
    }

    public void FlashStarRatingGreen()
    {
        SoundEffect.PlaySoundEffect(gainLife, transform.position, 1, soundGroup);
        StartCoroutine(FlashColorCoroutine(Color.green));
    }

    private IEnumerator FlashColorCoroutine(Color colorToFlash)
    {
        starRatingFill.color = colorToFlash;  // Sets to the desired color immediately.
        yield return new WaitForSeconds(0.2f);  // Flashes for half a second. Adjust this value as needed.
        starRatingFill.color = originalStarColor;  // Returns to original color.
    }

    public void TooltipDisplay()
    {
        if (currentWave == 1 && !gameObject.GetComponent<Wave>().waveInProgress)
        {
            toolText.text = "yo you beat wave 1";
            StartCoroutine(tooltipCoroutine());
            return;
        }
        if (currentWave == 2 && !gameObject.GetComponent<Wave>().waveInProgress)
        {
            toolText.text = "yo you beat wave 2";
            StartCoroutine(tooltipCoroutine());
            return;
        }
    }
    private IEnumerator tooltipCoroutine()
    {
        tooltip.SetActive(true);
        yield return new WaitForSeconds(1f);  // Flashes for half a second. Adjust this value as needed.
        tooltip.SetActive(false);
    }
}

