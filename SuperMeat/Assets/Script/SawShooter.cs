using UnityEngine;

public class SawShooter : MonoBehaviour
{
    public GameObject sawBladePrefab; // Assign SawBlade prefab in Inspector
    private GameObject _activeSawBlade; // Store reference to the currently spawned saw

    void Update()
    {
        // Check if the saw blade exists; if not, shoot a new one
        if (_activeSawBlade == null)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        _activeSawBlade = Instantiate(sawBladePrefab, transform.position, transform.rotation);
    }
}