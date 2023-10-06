using System.Collections;
using UnityEngine;

public class SpaceInvaderTower : Tower
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private float MovementSpeed = 1.0f;
    [SerializeField] private float sightDistance = 5.0f;
    [SerializeField] private Transform shipTransform;
    [SerializeField] private LineRenderer lineOfSight;

    public float SightDistance => sightDistance;

    private Vector2 leftBoundary;
    private Vector2 rightBoundary;

    private Wave waveScript;
    private Vector2 originalShipPosition;

    private bool enemyInSight = false;

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
            }

            if (EnemyInSight() && !enemyInSight)
            {
                enemyInSight = true;
                StartCoroutine(MoveShipLeftAndRight());
                StartCoroutine(ShootAtEnemies());
            }
        }
        else if (shipTransform.gameObject.activeSelf && enemyInSight)
        {
            enemyInSight = false;
            StopAllCoroutines();
            StartCoroutine(ReturnToOriginalPosition());
        }
    }

    private void DrawLineOfSight()
    {
        Vector3 endPosition = shipTransform.position - transform.up * sightDistance;
        lineOfSight.SetPosition(0, shipTransform.position);
        lineOfSight.SetPosition(1, endPosition);
    }

    private bool EnemyInSight()
    {
        int enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(shipTransform.position, -transform.up, sightDistance, enemyLayerMask);
        return hit.collider != null;
    }

    protected override void tryShoot()
    {
        if (waveScript.waveInProgress && enemyInSight)
        {
            int bulletCount = bulletLinesPerLevel[currentUpgradeLevel];
            float spacing = 0.2f;

            float startOffset = -spacing * (bulletCount - 1) / 2;

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

        
        bullet.GetComponent<Rigidbody2D>().velocity = -transform.up * 5;
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
    }

    private void CheckUpgrades()
    {
        if (currentUpgradeLevel != upgradeLevel)
        {
            currentUpgradeLevel = upgradeLevel;
        }
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, shipTransform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = -transform.up * 5;
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
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

        while (enemyInSight)
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
        while (enemyInSight)
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
