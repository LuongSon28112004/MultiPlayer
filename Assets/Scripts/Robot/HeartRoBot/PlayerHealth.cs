using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public int CurrentHealth { get => currentHealth; }

    // Sự kiện khi nhận damage
    public event Action<int> OnDamaged;

    public void TakeDamage(int amount)
    {
        if (!IsServer) return; // Chỉ Server mới có thể gọi hàm này 
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log("Player nhận " + amount+ " damage. Máu còn: " + currentHealth);
        // Gọi sự kiện
        if(OnDamaged == null) Debug.LogError("OnDamaged is null");
        OnDamaged?.Invoke(amount);
    }
}
