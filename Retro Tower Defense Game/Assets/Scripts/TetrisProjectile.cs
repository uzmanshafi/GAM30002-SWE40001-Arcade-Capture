using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TetrisProjectile : Projectile
{

    public TetrisTower owner; // This is instantiated on creation
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioMixerGroup soundGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void move()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                float damageMultiplier = owner.enemyHitEvent(e);
                SoundEffect.PlaySoundEffect(hit, transform.position, 1, soundGroup);
                e.TakeDamage(damage * damageMultiplier);
                Destroy(gameObject);
            }
        }
    }

    // What is the difference between this and OnCollisionEnter2D?
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo) {
                e.TakeDamage(damage);
            }
        }
    }
}
