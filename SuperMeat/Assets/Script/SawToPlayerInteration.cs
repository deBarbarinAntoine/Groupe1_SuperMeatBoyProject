using System.Collections;
using UnityEngine;

public class SawToPlayerInteration : MonoBehaviour
{
    public LayerMask sawBladeLayer; // Set to the saw blade's layer

    // Reference to the player GameObject (you may already have this reference)
    public PlayerController playerController;
    private Vector3 _initialPosition;
    private Rigidbody2D _rb;
    
    public float  detectionRadius = 0.1f; // Radius to check around the saw blade
    public float timePlayerIsFrozen = 1f;

    // private void OnDrawGizmos()
    // {
    //     // Set the Gizmo color (optional)
    //     Gizmos.color = Color.red;
    //
    //     // Draw a sphere at the GameObject's position with a radius of 1
    //     Gizmos.DrawSphere(transform.position, detectionRadius);
    // }
    private void Start()
    {
            // Save the initial position of the player at start
            _initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 boxCenter = transform.position; // Saw blade's position
        

        // Check if the sawblade is within the detection radius aka , player toucher by sawblade
        var hit = Physics2D.OverlapCircle(boxCenter, detectionRadius, sawBladeLayer);
        if (hit != null)
        {
            Debug.LogWarning("i'm hit");
            // Teleport player to its initial position
            transform.position = _initialPosition;

            // Freeze the player
            FreezePlayer();

            // Optionally, you could add a delay or condition to unfreeze the player
            StartCoroutine(UnfreezePlayer());
        }
    }

    private void FreezePlayer()
    {
        // Stop the player's movement by setting velocity to 0 and setting Rigidbody2D to Kinematic
        playerController.FreezePlayer();
    }

    private IEnumerator UnfreezePlayer()
    {
        yield return new WaitForSeconds(timePlayerIsFrozen); // Adjust the duration as needed
        playerController.UnfreezePlayer();
    }
}