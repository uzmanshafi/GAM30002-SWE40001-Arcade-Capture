using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField] private Vector3[] points;
    private Vector3 currentPosition;
    private bool gameStarted;

    public Vector3[] Points => points;
    public Vector3 CurrentPosition => currentPosition;
    

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getWaypointPosition(int index) //pass in waypoint index from enemy, index of which point enemy is up to
    {
        return currentPosition + points[index];
    }

    private void OnDrawGizmos()
    {
        if(!gameStarted && transform.hasChanged)
        {
            currentPosition = transform.position;
        }
        for (int i = 0; i < points.Length; i++ )
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(points[i] + currentPosition, 0.5f);

            if (i < points.Length - 1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(points[i] + currentPosition, points[i + 1] + currentPosition);
            }
        }


    }

}
