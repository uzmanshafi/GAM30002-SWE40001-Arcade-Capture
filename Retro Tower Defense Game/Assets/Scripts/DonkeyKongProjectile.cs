using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will go toward the nearest enemy. Once it arrives it starts rolling down the path towards the entrance. If the lifetimer or pierce is exceeded it is destroyed (Or reaches entrance)

public class DonkeyKongProjectile : Projectile
{

    private bool rollingAlongPath;
    private Vector2 destination;

    // This is how many enemies it can hit before it is destroyed
    [SerializeField] private int pierce;

    // Milliseconds, begins once on path (I don't know when this is instantiated because constructors don't seem to be a thing in unity)
    [SerializeField] private float barrelLifetime;

    // These are the points that the enemy has been to. We copy them here because the enemy could be destroyed
    private Vector3[] pathPoints;

    // This index starts at the end and goes towards 0.
    private int pathPointIndex;

    // Start is called before the first frame update
    void Start()
    {
        rollingAlongPath = false;
        destination = target.transform.position;

        // Save the path that the barrel will follow once it hits the path
        // Copy the waypoints that the enemy has gone through
        pathPoints = new Vector3[target.getWaypointIndex];
        System.Array.Copy(target.GetWaypoints.Points, 0, pathPoints, 0, target.getWaypointIndex);

        pathPointIndex = target.getWaypointIndex - 1;
    }

    // Update is called once per frame. (Does this call move?)
    void Update()
    {
        if (rollingAlongPath) {
            barrelLifetime -= Time.deltaTime;
            if (barrelLifetime <= 0) {
                Destroy(gameObject);
            }

            //Check if at point
            if (pointReached((Vector2)transform.position, (Vector2)pathPoints[pathPointIndex], 0.1f)) {
                pathPointIndex--;
                if (pathPointIndex <= 0) {
                    Debug.Log("End Reached!");
                    Destroy(gameObject);
                }
                destination = pathPoints[pathPointIndex];
                direction = (destination - (Vector2)transform.position).normalized;
                rotate();
            }
            else
            {
                move();
            }

        } else {
            if (target != null)
            {
                destination = target.transform.position;
            }
            //pointReached is going to be a globally accessible function. (Yes technically it could be an interface)
            if (pointReached((Vector2)transform.position, (Vector2)destination, 0.1f)) {
                // Currently this will just changes the state. Otherwise we could destroy this object and create a barrel object that does the same thing
                rollingAlongPath = true;
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                destination = pathPoints[pathPointIndex];
                direction = (destination - (Vector2)transform.position).normalized;
                rotate();
            }
        }

    }

    public override void move()
    {
        //Go towards destination. Note that we update destination if rollingAlongPath
        transform.position += (Vector3)(direction * speed) * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy e;
        if (collision.gameObject.TryGetComponent<Enemy>(out e))
        {
            e.TakeDamage(damage);
            pierce--;
            if (pierce <= 0)
            {
                Destroy(gameObject);
            }

        }
    }

    private void rotate()
    {
        transform.right = direction;
    }

    private bool pointReached(Vector2 position, Vector2 destination, float threshold = 0.01f) //will delegate to game manager when it exists
    {
        float distanceToNext = (position - destination).sqrMagnitude;
        if (distanceToNext <= threshold)
        {
            Debug.Log("PointReached!");
        }
        return distanceToNext <= threshold;
    }

}
