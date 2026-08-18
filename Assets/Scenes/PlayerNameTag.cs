using Unity.Netcode;
using TMPro;
using UnityEngine;
using Unity.Collections; // Required for FixedString

public class PlayerNameTag : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    // We use FixedString because strings can't be put directly into NetworkVariables
    private NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        // 1. Only the server assigns the name
        if (IsServer)
        {
            playerName.Value = "Player " + OwnerClientId;
        }

        // 2. Immediately update the text for current players
        nameText.text = playerName.Value.ToString();

        // 3. Listen for changes (in case names change mid-game)
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            nameText.text = newValue.ToString();
        };
    }
}