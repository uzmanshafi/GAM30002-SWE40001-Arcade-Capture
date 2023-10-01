using System.Collections;
using UnityEngine;

public class SpaceInvadersTower : Tower
{
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject bulletPrefab;
    private GameObject spawnedShip;
    private float movementSpeed = 1.0f;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private bool shipActive = false;
    private bool isShooting = false;
    private Wave waveScript;

    Vector2 towerDirection;

    void Awake()
    {
        waveScript = FindObjectOfType<Wave>();
        towerDirection = transform.up;
    }

    void Update()
    {
        if (waveScript && !waveScript.waveInProgress && shipActive)
        {

            StopCoroutine(MoveShipLeftAndRight());
            StopCoroutine(ShipShootStraight());
            isShooting = false;
        }
        else if (waveScript && waveScript.waveInProgress && !shipActive)
        {

            SpawnShip();
            shipActive = true;
            StartCoroutine(MoveShipLeftAndRight());
        }

        tryShoot();
    }

    protected override void tryShoot()
    {
        if (AnyEnemyInRange())
        {
            if (!shipActive)
            {
                SpawnShip();
                shipActive = true;
                StartCoroutine(MoveShipLeftAndRight());
            }
            if (!isShooting)
            {
                StartCoroutine(ShipShootStraight());
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }
    }

    private bool AnyEnemyInRange()
    {
        Enemy? enemy = furthestTarget();
        if (enemy != null)
        {
            Debug.Log("Enemy detected in range: " + enemy.name);
            return true;
        }
        return false;
    }


    private void SpawnShip()
    {
        Quaternion adjustedRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180);
        spawnedShip = Instantiate(shipPrefab, transform.position, adjustedRotation);

        // Determine local boundaries
        BoxCollider2D boundaryCollider = GetComponent<BoxCollider2D>();
        Vector2 boundarySize = boundaryCollider.size;

        Vector2 localLeftBoundary = -Vector2.right * boundarySize.x / 2;   // Left boundary in local coordinates
        Vector2 localRightBoundary = Vector2.right * boundarySize.x / 2;   // Right boundary in local coordinates

        // Convert local coordinates to world coordinates
        leftBoundary = transform.TransformPoint(localLeftBoundary);
        rightBoundary = transform.TransformPoint(localRightBoundary);
    }

    IEnumerator MoveShipLeftAndRight()
    {
        bool movingRight = true;

        while (shipActive)
        {
            if (movingRight)
            {
                while (Vector2.Distance(spawnedShip.transform.position, rightBoundary) > 0.1f)
                {
                    spawnedShip.transform.position = Vector2.MoveTowards(spawnedShip.transform.position, rightBoundary, movementSpeed * Time.deltaTime);
                    yield return null;
                }
                movingRight = false;
            }
            else
            {
                while (Vector2.Distance(spawnedShip.transform.position, leftBoundary) > 0.1f)
                {
                    spawnedShip.transform.position = Vector2.MoveTowards(spawnedShip.transform.position, leftBoundary, movementSpeed * Time.deltaTime);
                    yield return null;
                }
                movingRight = true;
            }
        }
    }

    IEnumerator ShipShootStraight()
    {
        Debug.Log("Bullet firing initiated.");
        while (isShooting)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnedShip.transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * 5;
            yield return new WaitForSeconds(cooldown);
        }
    }

}
