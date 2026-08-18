using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that touched the cube is a Player
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.FinishGameServerRpc();
        }
    }
}