using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class BasicProjectile : Projectile
{
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
            if (this.canSeeCamo || !e.IsCamo)
            {
                if ((e.TryGetComponent<Scroller>(out Scroller s) && s.colour != color && !canSeeCamo) || (e.TryGetComponent<Vandal>(out Vandal v) && !canSeeCamo2))
                {
                    Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
                }
                else
                {
                    e.TakeDamage(damage);
                    if (hit != null)
                    {
                        SoundEffect.PlaySoundEffect(hit, transform.position, 1, soundGroup);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            if (this.canSeeCamo || !e.IsCamo)
            {
                if ((e.TryGetComponent<Scroller>(out Scroller s) && s.colour != color && !canSeeCamo) || (e.TryGetComponent<Vandal>(out Vandal v) && !canSeeCamo2))
                {
                    Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision);
                }
                else
                {
                    e.TakeDamage(damage);
                    if (hit != null)
                    {
                        SoundEffect.PlaySoundEffect(hit, transform.position, 1, soundGroup);
                    }
                }

            }
        }
    }
}

