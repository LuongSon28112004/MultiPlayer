using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class BulletPoolManager : NetworkBehaviour
{
    public static BulletPoolManager Instance { get; private set; }

    [SerializeField] private NetworkObject bulletPrefab;
    [SerializeField] private List<NetworkObject> bulletPool = new List<NetworkObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private NetworkObject GetBulletFromPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.IsSpawned)
            {
                return bullet;
            }
        }

        // Nếu không có viên đạn sẵn, tạo mới
        NetworkObject newBullet = Instantiate(bulletPrefab);
        bulletPool.Add(newBullet);
        return newBullet;
    }

    public NetworkObject SpawnBullet(Vector3 position, Quaternion rotation, bool isLeft)
    {
        if (!IsServer) return null;

        NetworkObject bullet = GetBulletFromPool();

        if (!bullet.IsSpawned)
        {
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.gameObject.SetActive(true); // Phải Active trước khi Spawn
            bullet.Spawn(); // Chỉ Server gọi được
        }

        // Cập nhật hướng di chuyển
        Vector3 direction = isLeft ? Vector3.left : Vector3.right;
        UpdateBulletClientRpc(bullet.NetworkObjectId, direction, position);

        BulletFly bulletFly = bullet.GetComponent<BulletFly>();
        if (bulletFly != null)
            bulletFly.Direction = direction;

        return bullet;
    }

    public void DespawnBullet(NetworkObject bullet)
    {
        Debug.Log("DespawnBullet: " + bullet.NetworkObjectId);
        if (!IsServer || bullet == null) return;

        if (bullet.IsSpawned)
        {
            bullet.Despawn(false); // Giữ object, không phá hủy
            bullet.gameObject.SetActive(false); // Tắt gameobject
        }

        DeactivateBulletClientRpc(bullet.NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateBulletClientRpc(ulong bulletId, Vector3 direction, Vector3 position)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bulletId, out NetworkObject bullet))
        {
            bullet.transform.position = position;
            bullet.gameObject.SetActive(true);

            BulletFly bulletFly = bullet.GetComponent<BulletFly>();
            if (bulletFly != null)
                bulletFly.Direction = direction;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeactivateBulletClientRpc(ulong bulletId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bulletId, out NetworkObject bullet))
        {
            bullet.gameObject.SetActive(false);
        }
    }
}
