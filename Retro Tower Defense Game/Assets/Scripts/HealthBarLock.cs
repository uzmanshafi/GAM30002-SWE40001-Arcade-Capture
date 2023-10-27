using UnityEngine;

public class HealthBarLock : MonoBehaviour
{
    private Camera mainCamera;
    public Transform enemyTransform;
    public float heightOffset = 0.5f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            if (mainCamera != null && enemyTransform != null)
            {
                Vector3 offsetPosition = enemyTransform.position + Vector3.up * heightOffset;

                transform.position = offsetPosition;

                transform.LookAt(mainCamera.transform);
            }
        }
    }
}
