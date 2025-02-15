using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public GameObject playGroundCenter;
    public float smoothSpeed = 0.125f; // Camera smoothness
    public Vector3 offset; // Offset between player and camera

    public float xMin = -10f; // Minimum X bounds
    public float xMax = 10f; // Maximum X bounds
    public float yMin = -5f; // Minimum Y bounds
    public float yMax = 5f; // Maximum Y bounds

    private Vector3 _centerWorld = Vector3.zero;
    private float _width, _height;

    private void Start()
    {   
             _width = xMax - xMin;
             _height = yMax - yMin;
        var cam = GetComponent<Camera>();
        if (cam.orthographic)
        {
            var verticalSize = Mathf.Abs(yMax - yMin) / 2f;
            cam.orthographicSize = verticalSize;
        }
        
        if (playGroundCenter  != null)
        {
            // Get the center in world space
            _centerWorld = playGroundCenter .transform.position;
        }
        else
        {
            Debug.LogError("PlayGround not found");
        }
        
        if (player == null)
        {
            Debug.LogError("Player is not assigned to the MainCamera script.");
        }

    }

    private void LateUpdate()
    {
        
        var desiredPosition = player.transform.position + offset;
        desiredPosition.z = transform.position.z;

        
        // Clamp the X and Y based on _centerWorld
        var clampedX = Mathf.Clamp(desiredPosition.x, _centerWorld.x - _width / 2f, _centerWorld.x + _width / 2f);
        var clampedY = Mathf.Clamp(desiredPosition.y, _centerWorld.y - _height / 2f, _centerWorld.y + _height / 2f);

        // Apply the smoothed camera movement
        var smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }

}