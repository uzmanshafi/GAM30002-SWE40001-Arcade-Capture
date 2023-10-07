using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    private bool isDoubleSpeed = false;
    public GameObject light;

    public void ToggleGameSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;

        if (isDoubleSpeed)
        {
            // Activate double speed
            Debug.Log("x2 Speed enabled");
            light.SetActive(true);
            Time.timeScale = 2.0f;
        }
        else
        {
            // Restore normal speed
            Debug.Log("x2 Speed disabled");
            light.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}
