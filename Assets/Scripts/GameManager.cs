// using Unity.Netcode;

// public class GameManager : NetworkBehaviour
// {
//     public bool shouldSpawnPlayer = false;  // Biến này sẽ quyết định xem có spawn player hay không.

//     void Start()
//     {
//         if (shouldSpawnPlayer)
//         {
//             // Bắt đầu Host
//             NetworkManager.Singleton.StartHost();
//         }
//         else
//         {
//             // Bắt đầu Client mà không spawn player
//             NetworkManager.Singleton.StartClient();
//         }
//     }

//     public void SpawnPlayer()
//     {
//         if (shouldSpawnPlayer)
//         {
//             // Spawn player nếu cần thiết
//             NetworkManager.Singleton.SpawnManager.SpawnPlayerObject(NetworkManager.Singleton.LocalClientId);
//         }
//     }
// }
