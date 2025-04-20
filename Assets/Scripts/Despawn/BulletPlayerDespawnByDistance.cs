using Unity.Netcode;
using UnityEngine;

public class BulletPlayerDespawnByDistance : DespawnByDistance
{
    protected override void DeSpawnObject()
{
    var netObj = GetComponent<NetworkObject>();
    if (netObj != null)
    {
        BulletPoolManager.Instance.DespawnBullet(netObj);
    }
    else
    {
        Debug.LogWarning("NetworkObject not found on parent.");
    }
}


}
