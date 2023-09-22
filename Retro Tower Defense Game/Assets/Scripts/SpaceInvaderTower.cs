using System.Collections;
using UnityEngine;

public class SpaceInvadersTower : Tower
{
    [SerializeField] private float movementSpeed = 1.0f;
    private BoxCollider2D boundaryCollider;
    private Vector2 leftBoundary;
    private Vector2 rightBoundary;

    void Start()
    {
        base.init();
        boundaryCollider = GetComponent<BoxCollider2D>();
        Vector2 boundarySize = boundaryCollider.size;
        Vector2 center = boundaryCollider.offset;
        Debug.Log("SpaceInvadersTower initialized at: " + transform.position);
        leftBoundary = (Vector2)transform.position + center - new Vector2(boundarySize.x / 2, 0);
        rightBoundary = (Vector2)transform.position + center + new Vector2(boundarySize.x / 2, 0);

        InitializeBoundaries();
        StartCoroutine(MoveLeftAndRight());
        StartCoroutine(ShootStraight());
    }

    void Update()
    {
        Debug.Log("SpaceInvadersTower current position: " + transform.position);
    }


    protected override void tryShoot()
    {

    }

    public void InitializeBoundaries()
    {
        boundaryCollider = GetComponent<BoxCollider2D>();
        Vector2 boundarySize = boundaryCollider.size;
        Vector2 center = boundaryCollider.offset;
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad; // Convert angle to radians
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        leftBoundary = (Vector2)transform.position + center - direction * (boundarySize.x / 2);
        rightBoundary = (Vector2)transform.position + center + direction * (boundarySize.x / 2);
    }

    IEnumerator MoveLeftAndRight()
    {
        while (true)
        {

            while (Vector2.Distance(transform.position, rightBoundary) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, rightBoundary, movementSpeed * Time.deltaTime);
                yield return null;
            }


            InitializeBoundaries();


            while (Vector2.Distance(transform.position, leftBoundary) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, leftBoundary, movementSpeed * Time.deltaTime);
                yield return null;
            }


            InitializeBoundaries();
        }
    }

    IEnumerator ShootStraight()
    {
        while (true)
        {

            GameObject bullet = Instantiate(bulletTypes[0], transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * 5;


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
