using Unity.Netcode;
using UnityEngine;

public class CharacterSwitcher : NetworkBehaviour
{
    [SerializeField] private GameObject characterModel0;
    [SerializeField] private GameObject characterModel1;

    // Add these two slots in the Inspector
    [SerializeField] private Avatar avatar0;
    [SerializeField] private Avatar avatar1;

    public override void OnNetworkSpawn()
    {
        Animator animator = GetComponent<Animator>();

        if (OwnerClientId == 0)
        {
            characterModel0.SetActive(true);
            characterModel1.SetActive(false);
            animator.avatar = avatar0; // Uses the first girl's avatar
        }
        else
        {
            characterModel0.SetActive(false);
            characterModel1.SetActive(true);
            animator.avatar = avatar1; // Uses the second girl's avatar
        }
    }
}