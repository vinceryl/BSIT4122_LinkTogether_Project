using Unity.Netcode;
using UnityEngine;
using TMPro;
using StarterAssets;

public class ChatManager : NetworkBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;     // NEW SLOT
    [SerializeField] private GameObject terminalPanel; // NEW SLOT

    [Header("UI References")]
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatTextPrefab;
    [SerializeField] private Transform chatContent;

    [Header("Settings")]
    [SerializeField] private float chatCooldown = 1.2f;
    private float lastMessageTime;

    private StarterAssetsInputs playerInputs;

    public override void OnNetworkSpawn()
    {
        // Automatically swap panels when we enter the game
        if (menuPanel != null) menuPanel.SetActive(false);
        if (terminalPanel != null) terminalPanel.SetActive(true);

        chatInput.onSubmit.AddListener(OnSubmitChat);
    }

    void Update()
    {
        // Open chat with "/"
        if (Input.GetKeyDown(KeyCode.Slash) && !chatInput.isFocused)
        {
            chatInput.ActivateInputField();
            chatInput.text = "";
            TogglePlayerMovement(false);
        }

        // Escape to cancel
        if (Input.GetKeyDown(KeyCode.Escape) && chatInput.isFocused)
        {
            chatInput.DeactivateInputField();
            TogglePlayerMovement(true);
        }
    }

    private void OnSubmitChat(string message)
    {
        if (Time.time - lastMessageTime < chatCooldown)
        {
            ReceiveChatMessageClientRpc("System: Typing too fast!");
            chatInput.text = "";
            chatInput.DeactivateInputField();
            TogglePlayerMovement(true);
            return;
        }

        if (!string.IsNullOrEmpty(message))
        {
            SendChatMessageServerRpc(message);
            lastMessageTime = Time.time;
        }

        chatInput.DeactivateInputField();
        TogglePlayerMovement(true);
    }

    private void TogglePlayerMovement(bool canMove)
    {
        if (playerInputs == null) playerInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = canMove;
            playerInputs.cursorInputForLook = canMove;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        string playerId = "Player " + rpcParams.Receive.SenderClientId;
        ReceiveChatMessageClientRpc(playerId + ": " + message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string fullMessage)
    {
        TextMeshProUGUI newMessage = Instantiate(chatTextPrefab, chatContent);
        newMessage.text = fullMessage;
        Canvas.ForceUpdateCanvases();
    }
}