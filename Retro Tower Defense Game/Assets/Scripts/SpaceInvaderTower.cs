using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SpaceInvaderTower : Tower
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioMixerGroup soundGroup;
    [SerializeField] private float MovementSpeed = 1.0f;
    [SerializeField] private Transform shipTransform;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;

    private Wave waveScript;
    private Vector2 originalShipPosition;


    [Header("Level Values Configuration")]
    [SerializeField] private int[] bulletLinesPerLevel = { 1, 2, 3 };

    private int currentUpgradeLevel = 0;

    private void Awake()
    {
        waveScript = FindObjectOfType<Wave>();
        originalShipPosition = shipTransform.localPosition;
        shipTransform.gameObject.SetActive(false);
        SetBoundaries();
    }

    private void Update()
    {
        CheckUpgrades();

        if (waveScript.waveInProgress)
        {
            if (!shipTransform.gameObject.activeSelf)
            {
                shipTransform.gameObject.SetActive(true);
                StartCoroutine(AnimateLiftOff());
                StartCoroutine(MoveShipLeftAndRight());
                StartCoroutine(ShootAtEnemies());
            }
        }
        else if (shipTransform.gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(ReturnToOriginalPosition());
        }
    }

    protected override void tryShoot()
    {
        if (waveScript.waveInProgress)
        {
            int bulletCount = bulletLinesPerLevel[currentUpgradeLevel];
            float spacing = 0.2f;

            float startOffset = -spacing * (bulletCount - 1) / 2;
            
            SoundEffect.PlaySoundEffect(shootSound, transform.position, 1, soundGroup);

            for (int i = 0; i < bulletCount; i++)
            {
                float xOffset = startOffset + i * spacing;
                FireBullet(xOffset);
            }
        }
    }

    void FireBullet(float xOffset)
    {
        Vector3 spawnPosition = shipTransform.position + transform.right * xOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Projectile proj = bullet.GetComponent<Projectile>();
        proj.damage = damage;
        proj.color = controlColour;
        proj.canSeeCamo = canSeeCamo;
        proj.canSeeCamo2 = isStaffBuffed2;

        bullet.GetComponent<Rigidbody2D>().velocity = -transform.up * 5;
    }

    private void CheckUpgrades()
    {
        if (currentUpgradeLevel != upgradeLevel)
        {
            currentUpgradeLevel = upgradeLevel;
        }
    }

    private void SetBoundaries()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 size = collider.size;

        // Define the left and right boundaries in local coordinates
        leftBoundary = new Vector2(-size.x / 2, shipTransform.localPosition.y);
        rightBoundary = new Vector2(size.x / 2, shipTransform.localPosition.y);
    }

    IEnumerator AnimateLiftOff()
    {
        Vector3 targetScale = shipTransform.localScale;
        shipTransform.localScale = Vector3.zero;

        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            shipTransform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        shipTransform.localScale = targetScale;
    }

    IEnumerator MoveShipLeftAndRight()
    {
        bool movingRight = true;

        while (true)
        {
            Vector2 targetPosition = movingRight ? transform.TransformPoint(rightBoundary) : transform.TransformPoint(leftBoundary);

            while (Vector2.Distance(shipTransform.position, targetPosition) > 0.1f)
            {
                shipTransform.position = Vector2.MoveTowards(shipTransform.position, targetPosition, MovementSpeed * Time.deltaTime);
                yield return null;
            }

            movingRight = !movingRight;
            yield return null;
        }
    }

    IEnumerator ShootAtEnemies()
    {
        while (true)
        {
            tryShoot();
            yield return new WaitForSeconds(cooldown);
        }
    }

    IEnumerator ReturnToOriginalPosition()
    {
        while (Vector2.Distance(shipTransform.localPosition, originalShipPosition) > 0.1f)
        {
            shipTransform.localPosition = Vector2.MoveTowards(shipTransform.localPosition, originalShipPosition, MovementSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
