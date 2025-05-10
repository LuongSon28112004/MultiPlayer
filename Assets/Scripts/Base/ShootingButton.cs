using System;
using UnityEngine;

public class ShootingButton : BaseButton
{
    public event Action OnShootingButtonClicked;
    protected override void OnClick()
    {
        if (OnShootingButtonClicked == null)
        {
            Debug.Log("OnShootingButtonClicked is null");
            return;
        }
        Debug.Log("OnShootingButtonClicked is not null");
        OnShootingButtonClicked?.Invoke();
    }
}
