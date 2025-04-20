using System;
using Unity.Netcode;
using UnityEngine;

public abstract class Despawn : NetworkBehaviour
{
    protected virtual void FixedUpdate()
    {
        this.DeSpawning();
    }

    protected void DeSpawning()
    {
        if (!this.CanDespawn()) return;
        this.DeSpawnObject();
    }

    protected virtual void DeSpawnObject()
    {
        Destroy(transform.gameObject);
    }

    protected abstract bool CanDespawn();
}
