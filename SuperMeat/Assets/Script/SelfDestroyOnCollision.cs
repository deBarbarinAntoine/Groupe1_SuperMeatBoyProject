using UnityEngine;
using System.Collections;

public class SelfDestroyOnCollision : MonoBehaviour
{
    public float destroyDelay = 2f; // Delay before destruction

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}