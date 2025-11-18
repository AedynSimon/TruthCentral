using UnityEngine;

public class BillboardFace : MonoBehaviour
{
    // Optional: assign explicitly in Inspector.
    // If left empty, it will grab Camera.main.
    public Camera targetCamera;

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        // Make the front of the quad face the camera
        Vector3 dirToCamera = targetCamera.transform.position - transform.position;
        transform.forward = dirToCamera.normalized;
    }
}
