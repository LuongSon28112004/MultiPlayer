using Unity.Netcode;
using UnityEngine;

public class SpawnRobot : NetworkBehaviour
{
    public GameObject robotPrefab;

    void Start()
    {
        if (IsServer)
        {
            // Server spawn robot cho từng client (bao gồm chính nó)
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                SpawnRobotForClient(client.ClientId);
            }
        }
    }

    void SpawnRobotForClient(ulong clientId)
    {
        Vector3 spawnPosition = GetSpawnPosition(clientId);
        GameObject robot = Instantiate(robotPrefab, spawnPosition, Quaternion.identity);
        robot.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }

    Vector3 GetSpawnPosition(ulong clientId)
    {
        // Tuỳ logic: có thể dựa vào index hoặc random
        if (clientId == 0)
            return new Vector3(-2, 0, 0);
        else
            return new Vector3(2, 0, 0);
    }
}
