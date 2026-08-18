using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    void LateUpdate()
    {
        // Safety: Try to find the camera if it was lost
        if (mainCameraTransform == null)
        {
            if (Camera.main != null) mainCameraTransform = Camera.main.transform;
            return;
        }

        // Makes the UI face the camera perfectly
        transform.LookAt(transform.position + mainCameraTransform.forward);
    }
}