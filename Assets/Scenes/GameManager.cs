using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    private float timer = 0f;
    private bool isGameRunning = false;

    private void Awake() { Instance = this; }

    public override void OnNetworkSpawn()
    {
        if (IsServer) isGameRunning = true;
    }

    void Update()
    {
        if (isGameRunning)
        {
            timer += Time.deltaTime;
        }
    }

    // This is called when a player hits the finish line
    [ServerRpc(RequireOwnership = false)]
    public void FinishGameServerRpc()
    {
        if (!isGameRunning) return;

        isGameRunning = false;

        // Tell all clients to show the victory screen with the final time
        ShowVictoryClientRpc(timer);
    }

    [ClientRpc]
    void ShowVictoryClientRpc(float finalTime)
    {
        victoryPanel.SetActive(true);

        // Format the time into H:M:S
        TimeSpan t = TimeSpan.FromSeconds(finalTime);
        finalTimeText.text = string.Format("Time : {0:D2}h {1:D2}m {2:D2}s",
            t.Hours, t.Minutes, t.Seconds);

        // Unlock mouse so they can click continue
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}