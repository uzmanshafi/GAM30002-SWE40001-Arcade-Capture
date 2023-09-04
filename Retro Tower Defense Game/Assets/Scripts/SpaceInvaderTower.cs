using System.Collections;
using UnityEngine;

public class SpaceInvadersTower : Tower
{
    [SerializeField] private float movementSpeed = 1.0f;
    private BoxCollider2D boundaryCollider; // Reference to the BoxCollider2D component
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;

    void Start()
    {
        boundaryCollider = GetComponent<BoxCollider2D>();
        Vector2 boundarySize = boundaryCollider.size;
        Vector2 center = boundaryCollider.offset;
        Debug.Log("SpaceInvadersTower initialized at: " + transform.position);
        leftBoundary = (Vector2)transform.position + center - new Vector2(boundarySize.x / 2, 0);
        rightBoundary = (Vector2)transform.position + center + new Vector2(boundarySize.x / 2, 0);

        StartCoroutine(MoveLeftAndRight());
        StartCoroutine(ShootStraight());
    }

    void Update()
    {
        Debug.Log("SpaceInvadersTower current position: " + transform.position);
    }

    // Override the tryShoot method for specific shooting behavior
    protected override void tryShoot()
    {
        // Shooting logic is handled in the coroutine
    }

    IEnumerator MoveLeftAndRight()
    {
        while (true)
        {
            // Move to the right boundary
            while (Vector2.Distance(transform.position, rightBoundary) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, rightBoundary, movementSpeed * Time.deltaTime);
                yield return null;
            }

            // Move to the left boundary
            while (Vector2.Distance(transform.position, leftBoundary) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, leftBoundary, movementSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    IEnumerator ShootStraight()
    {
        while (true)
        {
            // Instantiate and shoot the bullet
            GameObject bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * 5; // replace 5 with the actual bullet speed

            // Wait for cooldown before next shot
            yield return new WaitForSeconds(cooldown);
        }
    }

    public void SetBoundaries()
    {
        Vector2 boundarySize = boundaryCollider.size;
        Vector2 center = boundaryCollider.offset;
        leftBoundary = (Vector2)transform.position + center - new Vector2(boundarySize.x / 2, 0);
        rightBoundary = (Vector2)transform.position + center + new Vector2(boundarySize.x / 2, 0);
    }

}
