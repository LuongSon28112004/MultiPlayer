using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class NetworkUIManager : MonoBehaviour
{
    [Header("Host")]
    public TMP_InputField hostNameInput;
    public TMP_InputField hostPortInput;
    public Button startHostButton;
    public Button stopHostButton;

    [Header("Client")]
    public TMP_InputField clientIPInput;
    public TMP_InputField clientPortInput;
    public Button startClientButton;

    [Header("Status")]
    public TMP_Text statusText;

    private NetworkVariable<int> playerCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    void Start()
    {
        Debug.Log("NetworkUIManager Start");

        startHostButton.onClick.AddListener(OnStartHostClicked);
        stopHostButton.onClick.AddListener(OnStopHostClicked);
        startClientButton.onClick.AddListener(OnStartClientClicked);

        statusText.text = "Not connected.";

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            playerCount.OnValueChanged += OnPlayerCountChanged;

            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }

        playerCount.OnValueChanged -= OnPlayerCountChanged;
    }

    void OnStartHostClicked()
    {
        Debug.Log("Start Host clicked");

        if (NetworkManager.Singleton == null) return;

        string localIP = GetLocalIPv4();
        hostNameInput.text = localIP;

        string portStr = hostPortInput.text.Trim();
        ushort port = string.IsNullOrEmpty(portStr) ? (ushort)7777 : ushort.Parse(portStr);
        hostPortInput.text = port.ToString();

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = "0.0.0.0";
        transport.ConnectionData.Port = port;

        // Tắt PlayerPrefab để không spawn ở menu
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;

        if (NetworkManager.Singleton.StartHost())
        {
            statusText.text = $"Hosting on {localIP}:{port}...";
            playerCount.Value = 1;
        }
        else
        {
            statusText.text = "Failed to start Host.";
        }
    }

    void OnStopHostClicked()
    {
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            statusText.text = "Host stopped.";
            playerCount.Value = 0;
        }
    }

    void OnStartClientClicked()
    {
        if (NetworkManager.Singleton == null) return;

        string ip = clientIPInput.text.Trim();
        string portStr = clientPortInput.text.Trim();

        if (!ushort.TryParse(portStr, out ushort port))
        {
            statusText.text = "Invalid port.";
            return;
        }

        if (string.IsNullOrEmpty(ip))
        {
            statusText.text = "IP address is required.";
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;

        // Tắt PlayerPrefab để không spawn sớm
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null;

        statusText.text = $"Connecting to {ip}:{port}...";
        if (!NetworkManager.Singleton.StartClient())
        {
            statusText.text = "Failed to connect as client.";
        }
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            playerCount.Value++;
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = NetworkManager.Singleton.IsHost ? "Hosting..." : "Connected as Client";
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            playerCount.Value = Mathf.Max(0, playerCount.Value - 1);
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = "Disconnected.";
        }
    }

    void OnPlayerCountChanged(int oldValue, int newValue)
    {
        Debug.Log($"Player count changed: {newValue}");

        if (NetworkManager.Singleton.IsServer && newValue >= 2)
        {
            StartCoroutine(LoadLobbyScene());
        }
    }

    IEnumerator LoadLobbyScene()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Enough players! Loading Lobby scene...");

        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadMode)
    {
        Debug.Log($"Client {clientId} loaded scene: {sceneName}");

        // Chỉ spawn PlayerPrefab trong Lobby
        if (sceneName == "Lobby" && NetworkManager.Singleton.IsServer)
        {
            var playerPrefab = Resources.Load<GameObject>("Prefabs/PlayerRobot");
            if (playerPrefab != null)
            {
                NetworkManager.Singleton.NetworkConfig.PlayerPrefab = playerPrefab;
                Debug.Log("PlayerPrefab set in Lobby");
            }
            else
            {
                Debug.LogError("PlayerRobot prefab not found in Resources/Prefabs!");
            }
        }
    }

    string GetLocalIPv4()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }
}
