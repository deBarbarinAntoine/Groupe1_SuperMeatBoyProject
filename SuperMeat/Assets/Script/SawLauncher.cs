using UnityEngine;

public class SawLauncher : MonoBehaviour
{
    [Header("Launcher Settings")]
    public GameObject sawPrefab;       // The saw prefab to spawn
    public Transform launchPoint;      // Where the saw will spawn
    public float launchForce = 10f;    // Speed of the launched saw
    public float fireRate = 2f;        // Time between saw launches

    [Header("Saw Lifetime")]
    public float sawLifetime = 5f;     // How long the saw exists before being destroyed

    private float _fireCooldown;

    private void Update()
    {
        // Check if enough time has passed to launch another saw
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            LaunchSaw();
            _fireCooldown = fireRate; // Reset cooldown
        }
    }

    private void LaunchSaw()
    {
        // Instantiate the saw at the launch point
        GameObject newSaw = Instantiate(sawPrefab, launchPoint.position, launchPoint.rotation);

        // Apply force to the saw (assumes Rigidbody2D)
        Rigidbody2D rb = newSaw.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(launchPoint.right * launchForce, ForceMode2D.Impulse); // Launch in the forward direction
        }

        // Destroy the saw after a set time
        Destroy(newSaw, sawLifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Replace with appropriate tag
        {
            // Apply damage or other effects
            Debug.Log("Player hit by saw!");
        }

        // Optionally destroy the saw on collision
        Destroy(gameObject);
    }

}