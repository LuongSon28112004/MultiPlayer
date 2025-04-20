using Unity.Netcode;
using UnityEngine;

public class BulletPlayerDespawnByCollider : DespawnByCollider
{
    private void OnCollisionEnter2D(Collision2D other)
    {
         Debug.Log("BulletPlayerDespawnByCollider: " + other.gameObject.name);
        if (other.gameObject.CompareTag("PlayerRobot"))
        {
            Debug.Log("ok");
            if (!IsServer) return;
            this.isDeSpawn = true;

            var playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            else
            {
                Debug.Log("PlayerHealth component not found on the collided object.");
            }
        }
    }

    protected override void DeSpawnObject()
    {
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null)
        {
            BulletPoolManager.Instance.DespawnBullet(netObj);
        }
    }
}
