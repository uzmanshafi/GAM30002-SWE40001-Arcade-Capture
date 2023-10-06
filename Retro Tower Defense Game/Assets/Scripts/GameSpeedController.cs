using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private bool isDoubleSpeed = false;

    public void ToggleGameSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;

        if (isDoubleSpeed)
        {
            // Activate double speed
            Debug.Log("x2 Speed enabled");
            Time.timeScale = 2.0f;
        }
        else
        {
            // Restore normal speed
            Debug.Log("x2 Speed disabled");
            Time.timeScale = 1.0f;
        }
    }
}
