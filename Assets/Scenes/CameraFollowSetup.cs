using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraFollowSetup : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // 1. Link the Camera to the Player Input
            if (TryGetComponent(out PlayerInput playerInput))
            {
                playerInput.camera = Camera.main;
            }

            // 2. Find and setup the Virtual Camera
            var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                Transform cameraRoot = transform.Find("PlayerCameraRoot");
                if (cameraRoot != null)
                {
                    vcam.Follow = cameraRoot;
                    vcam.LookAt = cameraRoot;

                    // Force the camera to become active
                    vcam.Priority = 100;
                }
            }
        }
    }
}