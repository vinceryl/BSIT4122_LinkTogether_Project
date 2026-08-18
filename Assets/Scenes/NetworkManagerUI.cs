using Unity.Netcode;
using Unity.Netcode.Transports.UTP; // Required to change the IP address
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button localBtn;
    [SerializeField] private Button soloBtn;

    [Header("Settings")]
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject menuPanel;

    private void Awake()
    {
        // 1. HOST A GAME: Starts server and waits for partner
        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            menuPanel.SetActive(false);
        });

        // 2. JOIN A GAME: Connects to the IP typed in the box
        joinBtn.onClick.AddListener(() =>
        {
            string targetIP = ipInputField.text;
            if (string.IsNullOrEmpty(targetIP)) targetIP = "127.0.0.1"; // Default to self

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = targetIP;

            NetworkManager.Singleton.StartClient();
            menuPanel.SetActive(false);
        });

        // 3. LOCAL MULTIPLAYER: Automatically connects to self (for testing 2 windows)
        localBtn.onClick.AddListener(() =>
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = "127.0.0.1";
            NetworkManager.Singleton.StartClient();
            menuPanel.SetActive(false);
        });

        // 4. TRAINING MODE (SOLO): Starts game with no networking needed
        soloBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost(); // Start as host so you exist in world
            menuPanel.SetActive(false);
            Debug.Log("Solo Mode Started");
        });
    }
    // Add this inside the NetworkManagerUI class
    public void BackToMainMenu()
    {
        // 1. Shut down the network connection (Host or Client)
        NetworkManager.Singleton.Shutdown();

        // 2. Show the Main Menu again
        menuPanel.SetActive(true);

        // 3. Find the Victory Panel and hide it
        // (We find it by name since it's likely hidden)
        GameObject vicPanel = GameObject.Find("Canvas").transform.Find("VictoryPanel").gameObject;
        if (vicPanel != null) vicPanel.SetActive(false);

        // 4. Ensure the mouse is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Returned to Main Menu");
    }
}