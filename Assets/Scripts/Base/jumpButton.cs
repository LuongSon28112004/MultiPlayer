using System;
using UnityEngine;

public class jumpButton : BaseButton
{
   public event Action OnJumpButtonClicked;

    protected override void OnClick()
    {
        OnJumpButtonClicked?.Invoke();
    }
}
