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
        if (playerHealth != null)
        playerHealth.OnDamaged += UpdateUI;
    }

    private void addOnChangeVariable()
    {
        health.OnValueChanged += (oldValue, newValue) =>
        {
            // if (!IsOwner)
            // {
                Debug.Log("HealthUI: " + newValue);
                healthText.text = newValue.ToString();
            //}
        };
    }

    void UpdateUI()
    {
        if (IsOwner) healthText.text = playerHealth.CurrentHealth.ToString();
        if (IsHost || IsServer)
        {
            health.Value = playerHealth.CurrentHealth;
        }
    }
}
