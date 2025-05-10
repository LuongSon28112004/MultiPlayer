using System;
using Unity.Netcode;
using UnityEngine;

public class MoveLeftButton : BaseButton
{
    
    public event Action OnMoveLeftButtonClicked;
     protected override void OnClick()
    {
        if(OnMoveLeftButtonClicked == null)
        {
            Debug.Log("OnMoveLeftButtonClicked is null");
            return;
        }
        Debug.Log("OnMoveLeftButtonClicked is not null");
        OnMoveLeftButtonClicked?.Invoke();

    }

}

