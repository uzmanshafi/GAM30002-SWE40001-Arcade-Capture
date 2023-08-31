using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 0.5f;     
    public float zoomSpeed = 10.0f;   
    public float minZoom = 2.0f;      
    public float maxZoom = 10.0f;     

    private Vector3 initialPosition;
    private Camera myCamera;

    void Start()
    {

        initialPosition = transform.position;
        myCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
  
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        float zoomAmount = scrollData * zoomSpeed * Time.deltaTime;
        float newZoom = Mathf.Clamp(myCamera.orthographicSize - zoomAmount, minZoom, maxZoom);
        myCamera.orthographicSize = newZoom;


        if (Input.GetMouseButton(2))  
        {
            Vector3 pos = myCamera.ScreenToViewportPoint(Input.mousePosition - myCamera.transform.position);
            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.World);  
        }
    }
}
