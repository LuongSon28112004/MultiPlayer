// Connection.cs
using UnityEngine;
using Unity.Netcode;

public class Connection : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true; // Chấp nhận tất cả kết nối
        response.CreatePlayerObject = false; // Đảm bảo spawn PlayerPrefab khi client kết nối
        response.Pending = false;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }
}
