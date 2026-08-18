using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem; // Required for PlayerInput
using StarterAssets; // Required for the ThirdPersonController components

public class NetworkInputFix : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // If I am NOT the person controlling this character
        if (!IsOwner)
        {
            // Disable the components that listen to the keyboard
            if (TryGetComponent(out PlayerInput playerInput)) playerInput.enabled = false;
            if (TryGetComponent(out StarterAssetsInputs inputs)) inputs.enabled = false;

            // Optional: Disable the controller to save performance on the clone
            if (TryGetComponent(out ThirdPersonController controller)) controller.enabled = false;
        }
    }
}