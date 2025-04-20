using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class CameraFolowingPlayer : NetworkBehaviour
{

    [SerializeField] private Transform playerTransform;
    private bool isLoading = false;

    void Update()
    {
        if (!IsOwner && isLoading) 
        {
            return;
        }


        // Tìm Player có NetworkObject là của mình
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkObject>().IsOwner)
            {
                playerTransform = player.transform;
                isLoading = true;
                break;
            }
        }
    }

    void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 newPos = playerTransform.position + new Vector3(0, 2, -5); // Điều chỉnh vị trí phù hợp
            transform.position = newPos;
            //transform.LookAt(playerTransform);
        }
    }
}
