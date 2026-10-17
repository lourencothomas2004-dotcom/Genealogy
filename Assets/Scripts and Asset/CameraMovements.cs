using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float panSpeed = 100f;
    public float scrollSpeed = 10000f;
    public float minZ = -1000000f;
    public float maxZ = 1f;

    void Update()
    {
        Vector3 pos = transform.position;

        // Handle X and Y movement (Panning)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        
        pos.x += moveX * panSpeed * Time.deltaTime;
        pos.y += moveY * panSpeed * Time.deltaTime;

        // Handle Z movement (Zooming)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (Mathf.Abs(scroll) > 0f)
        {
            pos.z += scroll * scrollSpeed * Time.deltaTime;
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        }

        transform.position = pos;
    }
}