using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public GameObject playGround;
    public float smoothSpeed = 0.125f; // Camera smoothness
    public Vector3 offset; // Offset between player and camera

    public float xMin = -10f; // Minimum X bounds
    public float xMax = 10f; // Maximum X bounds
    public float yMin = -5f; // Minimum Y bounds
    public float yMax = 5f; // Maximum Y bounds

    private Vector3 _centerWorld = Vector3.zero;

    private void Start()
    {
        var cam = GetComponent<Camera>();
        if (cam.orthographic)
        {
            var verticalSize = Mathf.Abs(yMax - yMin) / 2f;
            cam.orthographicSize = verticalSize;
        }

        var playGroundCollider = playGround.GetComponent<BoxCollider2D>();
        if (playGroundCollider != null)
        {
            // Get the center in world space
            _centerWorld = playGround.transform.TransformPoint(playGroundCollider.offset);
        }
        else
        {
            Debug.LogWarning("PlayGround not found");
        }
    }

    private void LateUpdate()
    {
        
        var desiredPosition = player.transform.position + offset;
        desiredPosition.z = transform.position.z;

        // Calculate the relative clamping bounds using _centerWorld as the center
        float width = xMax - xMin;
        float height = yMax - yMin;

        // Clamp the X and Y based on _centerWorld
        var clampedX = Mathf.Clamp(desiredPosition.x, _centerWorld.x - width / 2f, _centerWorld.x + width / 2f);
        var clampedY = Mathf.Clamp(desiredPosition.y, _centerWorld.y - height / 2f, _centerWorld.y + height / 2f);

        // Apply the smoothed camera movement
        var smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }

}