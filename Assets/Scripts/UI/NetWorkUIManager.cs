// NetworkUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

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
    public TMP_Text countClient;

    private const int MinClientToStartLobby = 2;
    private const ushort DefaultPort = 7777;
    private const string LobbySceneName = "Lobby";
    [SerializeField] private GameObject playerPrefab;

    private bool sceneManagerHooked = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("NetworkUIManager Start");

        startHostButton.onClick.AddListener(OnStartHostClicked);
        stopHostButton.onClick.AddListener(OnStopHostClicked);
        startClientButton.onClick.AddListener(OnStartClientClicked);

        statusText.text = "Not connected.";

        // Hook scene manager before any other setup
        if (NetworkManager.Singleton != null)
        {
            HookSceneManager();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    void Update()
    {
        // Đảm bảo hook SceneManager khi NetworkManager sẵn sàng
        if (!sceneManagerHooked && NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            HookSceneManager();
        }
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            if (sceneManagerHooked && NetworkManager.Singleton.SceneManager != null)
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }
    }

    void OnStartHostClicked()
    {
        if (NetworkManager.Singleton == null) return;

        string localIP = GetLocalIPv4();
        hostNameInput.text = localIP;

        ushort port = TryGetPortFromInput(hostPortInput.text, DefaultPort);
        hostPortInput.text = port.ToString();

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = localIP;
        transport.ConnectionData.Port = port;

        // Ensure HookSceneManager is called before starting host
        if (NetworkManager.Singleton.StartHost())
        {
            statusText.text = $"Hosting on {localIP}:{port}";
        }
        else
        {
            statusText.text = "Failed to start Host.";
        }
    }

    void OnStopHostClicked()
    {
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
        {
            NetworkManager.Singleton.Shutdown();
            statusText.text = "Host stopped.";
        }
    }

    void OnStartClientClicked()
    {
        if (NetworkManager.Singleton == null) return;

        string ip = clientIPInput.text.Trim();
        string portStr = clientPortInput.text.Trim();

        if (!IPAddress.TryParse(ip, out _))
        {
            statusText.text = "Invalid IP format.";
            return;
        }

        if (!ushort.TryParse(portStr, out ushort port))
        {
            statusText.text = "Invalid port format.";
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;
        transport.ConnectionData.Port = port;

        statusText.text = $"Connecting to {ip}:{port}...";
        if (NetworkManager.Singleton.StartClient())
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedFeedback;
        }
        else
        {
            statusText.text = "Failed to start client.";
        }
    }

    void HookSceneManager()
    {
        if (!sceneManagerHooked && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            sceneManagerHooked = true;
        }
    }

    void OnClientDisconnectedFeedback(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = "Failed to connect. Check IP/Port or Host.";
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedFeedback;
        }
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            int count = NetworkManager.Singleton.ConnectedClientsList.Count;
            UpdateClientCountClientRpc(count);

            if (count >= MinClientToStartLobby && SceneManager.GetActiveScene().name != LobbySceneName)
            {
                StartCoroutine(LoadLobbyScene());
            }
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = NetworkManager.Singleton.IsHost ? "Hosting..." : "Connected as Client";
            RequestClientCountServerRpc();
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");

        if (NetworkManager.Singleton.IsServer)
        {
            int count = NetworkManager.Singleton.ConnectedClientsList.Count;
            UpdateClientCountClientRpc(count);
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            statusText.text = "Disconnected.";
        }
    }

    IEnumerator LoadLobbyScene()
    {
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.SceneManager.LoadScene(LobbySceneName, LoadSceneMode.Single);
    }

    void OnLoadEventCompleted(string sceneName, LoadSceneMode mode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        foreach (var client in clientsCompleted)
        {
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(client)) continue;

            if (NetworkManager.Singleton.ConnectedClients[client].PlayerObject == null)
            {
                GameObject player = Instantiate(playerPrefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client);
                Debug.Log($"[Server] Spawned player for client {client}");
            }
        }
    }


    string GetLocalIPv4()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }
        return "127.0.0.1";
    }

    ushort TryGetPortFromInput(string portInput, ushort fallback)
    {
        return ushort.TryParse(portInput, out ushort port) ? port : fallback;
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestClientCountServerRpc(ServerRpcParams rpcParams = default)
    {
        int count = NetworkManager.Singleton.ConnectedClientsList.Count;
        UpdateClientCountClientRpc(count);
    }

    [ClientRpc]
    void UpdateClientCountClientRpc(int count)
    {
        if (countClient != null)
            countClient.text = $"Connected Clients: {count}";
    }
}

