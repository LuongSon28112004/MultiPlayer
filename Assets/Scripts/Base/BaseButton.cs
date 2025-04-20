using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void AddEventOnClick()
    {
        button.onClick.AddListener(OnClick);
    }

    protected abstract void OnClick();
}
