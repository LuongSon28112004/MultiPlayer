using System;
using UnityEngine;

public class DespawnByDistance : Despawn
{
    [SerializeField] protected float disLimit = 40.0f;
    [SerializeField] protected float distance = 0.0f;
    [SerializeField] protected Transform mainCamera;

    void Awake()
    {
        this.LoadCamera();
    }

    private void LoadCamera()
    {
        if (this.mainCamera != null) return;
        this.mainCamera = Transform.FindAnyObjectByType<Camera>().transform;
        //Debug.Log(transform.parent.name + " Load Camera" + gameObject);
    }

    protected override bool CanDespawn()
    {
        this.distance = Vector3.Distance(transform.position, this.mainCamera.transform.position);
        if (this.distance > disLimit) return true;
        return false;
    }
}
