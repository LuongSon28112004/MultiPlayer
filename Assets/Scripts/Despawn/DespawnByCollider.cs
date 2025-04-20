using UnityEngine;

public class DespawnByCollider : Despawn
{
    [SerializeField] protected bool isDeSpawn = false;

    protected override bool CanDespawn()
    {
        return isDeSpawn;
    }

    

    public override void OnNetworkSpawn()
    {
        isDeSpawn = false;
        base.OnNetworkSpawn();
        ResetPhysics();
    }

    private void ResetPhysics()
    {
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null && !collider.enabled)
            collider.enabled = true;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null && !rb.simulated)
            rb.simulated = true;
    }
}
