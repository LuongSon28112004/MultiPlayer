using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public int CurrentHealth { get => currentHealth; }

    // Sự kiện khi nhận damage
    public event Action OnDamaged;

    void Update()
    {
        //if(OnDamaged == null) Debug.LogError("OnDamaged is null");
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return; // Chỉ Server mới có thể gọi hàm này 
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log("Player nhận " + amount + " damage. Máu còn: " + currentHealth);
        // Gọi sự kiện
        if (OnDamaged == null) Debug.LogError("OnDamaged is null");
        OnDamaged?.Invoke();
        if(currentHealth <= 0)
        {
            // Gọi hàm chết ở đây
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject); // Hoặc thực hiện hành
    }
}
