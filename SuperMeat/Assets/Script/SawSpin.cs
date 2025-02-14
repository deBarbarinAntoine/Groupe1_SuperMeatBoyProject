using UnityEngine;

public class SawSpin : MonoBehaviour
{
    public float spinSpeed = -270f; // Degrees per second

    private void Update()
    {
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }
}

