using UnityEngine;

public class Loading : MonoBehaviour {
    public Vector3 rotationAxis = Vector3.forward; // Z-axis (like loading spinner)
    public float rotationSpeed = 100f;

    void Update() {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
