using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInvadersTower : Tower
{
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioClip shoot;
    private GameObject spawnedShip;
    private float movementSpeed = 1.0f;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;
    private bool shipActive = false;
    private bool isShooting = false;
    private Wave waveScript;

    private List<GameObject> spawnedBullets = new List<GameObject>(); // List to track bullets

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
            shipActive = false;
            StopCoroutine(MoveShipLeftAndRight());
            StopCoroutine(ShipShootStraight());
            isShooting = false;
        }
        else if
            (waveScript && waveScript.waveInProgress && !shipActive)
        {
            SpawnShip();
            shipActive = true;
            StartCoroutine(MoveShipLeftAndRight());
            StartCoroutine(ShipShootStraight());
        }

        if (waveScript && waveScript.waveInProgress && !isShooting)
        {
            isShooting = true;
            StartCoroutine(ShipShootStraight());
        }
    }

    protected override void tryShoot()
    {

    }

    private void SpawnShip()
    {
        Quaternion adjustedRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180);
        spawnedShip = Instantiate(shipPrefab, transform.position, adjustedRotation);

        towerDirection = adjustedRotation * Vector2.up;

        BoxCollider2D boundaryCollider = GetComponent<BoxCollider2D>();
        Vector2 boundarySize = boundaryCollider.size;

        Vector2 localLeftBoundary = -Vector2.right * boundarySize.x / 2;
        Vector2 localRightBoundary = Vector2.right * boundarySize.x / 2;

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
            AudioSource.PlayClipAtPoint(shoot, transform.position);
            GameObject bullet = Instantiate(bulletPrefab, spawnedShip.transform.position, Quaternion.identity);
            spawnedBullets.Add(bullet);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = towerDirection * 5;
            yield return new WaitForSeconds(cooldown);
        }
    }


    private void OnDestroy()
    {
        if (spawnedShip != null)
        {
            Destroy(spawnedShip);
        }

        foreach (var bullet in spawnedBullets)
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }
    }
}
