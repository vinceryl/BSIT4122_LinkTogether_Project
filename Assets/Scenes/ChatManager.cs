using Unity.Netcode;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;

public class ChatManager : NetworkBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject terminalPanel;

    [Header("Terminal Elements")]
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatTextPrefab;
    [SerializeField] private Transform chatContent;

    [Header("Anti-Spam Settings")]
    [SerializeField] private float chatCooldown = 1.0f;
    private float lastMessageTime;

    private StarterAssetsInputs playerInputs;

    public override void OnNetworkSpawn()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (terminalPanel != null) terminalPanel.SetActive(true);

        chatInput.onSubmit.AddListener(OnSubmitChat);

        SendSystemAlertServerRpc($"Robot_{OwnerClientId} has established a secure link.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash) && !chatInput.isFocused)
        {
            TogglePlayerControls(false);
            chatInput.ActivateInputField();
            chatInput.text = "";
        }

        if (Input.GetKeyDown(KeyCode.Escape) && chatInput.isFocused)
        {
            chatInput.DeactivateInputField();
            TogglePlayerControls(true);
        }
    }

    private void OnSubmitChat(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        if (Time.time - lastMessageTime < chatCooldown)
        {
            AddLocalLog("<color=red>System: You are sending messages too fast!</color>");
            chatInput.text = "";
            chatInput.DeactivateInputField();
            TogglePlayerControls(true);
            return;
        }
        lastMessageTime = Time.time;

        if (input.StartsWith("/w ")) { HandleWhisper(input); }
        else if (input.ToLower() == "/help") { AddLocalLog("<color=yellow>[HELP]</color> /w [id] [msg], /clear"); }
        else if (input.ToLower() == "/clear") { foreach (Transform child in chatContent) Destroy(child.gameObject); }
        else { SendChatMessageServerRpc(input); }

        chatInput.text = "";
        chatInput.DeactivateInputField();
        TogglePlayerControls(true);
    }

    private void HandleWhisper(string input)
    {
        string[] parts = input.Split(' ', 3);
        if (parts.Length < 3) return;
        if (ulong.TryParse(parts[1], out ulong targetId))
        {
            SendWhisperServerRpc(targetId, parts[2]);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendSystemAlertServerRpc(string message)
    {
        ReceiveChatMessageClientRpc($"[SYSTEM] {message}", "#00FFFF");
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
        ReceiveChatMessageClientRpc($"Robot_{senderId}: {message}", "white");
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string fullMessage, string color)
    {
        AddLocalLog($"<color={color}>{fullMessage}</color>");
    }

    [ServerRpc(RequireOwnership = false)]
    void SendWhisperServerRpc(ulong targetId, string message, ServerRpcParams rpcParams = default)
    {
        ReceiveWhisperClientRpc(targetId, rpcParams.Receive.SenderClientId, message);
    }

    [ClientRpc]
    void ReceiveWhisperClientRpc(ulong targetId, ulong senderId, string message)
    {
        if (NetworkManager.Singleton.LocalClientId == targetId || NetworkManager.Singleton.LocalClientId == senderId)
        {
            AddLocalLog($"<color=#FF00FF>[WHISPER from Robot_{senderId}]: {message}</color>");
        }
    }

    private void AddLocalLog(string text)
    {
        var newMessage = Instantiate(chatTextPrefab, chatContent);
        newMessage.text = text;
        Canvas.ForceUpdateCanvases();
    }

    private void TogglePlayerControls(bool isGameplayMode)
    {
        if (playerInputs == null) playerInputs = FindAnyObjectByType<StarterAssetsInputs>();
        if (playerInputs != null)
        {
            Cursor.lockState = isGameplayMode ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isGameplayMode;
            playerInputs.cursorInputForLook = isGameplayMode;
            if (!isGameplayMode) { playerInputs.move = Vector2.zero; playerInputs.jump = false; }
        }
    }
}