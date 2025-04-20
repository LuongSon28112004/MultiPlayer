using Unity.Netcode;
using UnityEngine;

public class BulletFly : NetworkBehaviour, Fly
{
    [SerializeField] protected float moveSpeed = 5;
    [SerializeField] private Vector3 direction = Vector3.right;

    public Vector3 Direction { get => direction; set => direction = value; }

    void Update()
    {
        if (!IsServer) return;
        this.Move();
    }
    public void Move()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

}
