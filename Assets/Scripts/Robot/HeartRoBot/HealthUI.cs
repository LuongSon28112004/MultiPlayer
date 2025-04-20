using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HealthUI : NetworkBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private NetworkVariable<int> health = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealth>();
        }

        if (healthText == null)
        {
            healthText = GetComponent<TextMeshProUGUI>();
        }

        addOnChangeVariable();
    }

    private void addOnChangeVariable()
    {
        health.OnValueChanged += (oldValue, newValue) =>
        {
            if (!IsOwner)
            {
                Debug.Log("HealthUI: " + newValue);
                healthText.text = newValue.ToString();
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        if (playerHealth != null)
        {
            Debug.Log("Add UpdateUI");
            playerHealth.OnDamaged += UpdateUI;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (playerHealth != null)
            playerHealth.OnDamaged -= UpdateUI;
    }
    
    void UpdateUI(int damage)
    {
        if (IsOwner) healthText.text = playerHealth.CurrentHealth.ToString();
        if (IsHost || IsServer)
        {
            health.Value = playerHealth.CurrentHealth;
        }
        else
        {
            sendHealthServerRpc(playerHealth.CurrentHealth);
        }
    }

    [Rpc(SendTo.Server)]
    private void sendHealthServerRpc(int health)
    {
        this.health.Value = health;
    }
}
