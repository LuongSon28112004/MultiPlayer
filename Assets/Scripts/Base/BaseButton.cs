using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : NetworkBehaviour
{
    [SerializeField] private Button button;
    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        AddEventOnClick();
    }

    public void AddEventOnClick()
    {
        button.onClick.AddListener(OnClick);
    }

    protected abstract void OnClick();
}
