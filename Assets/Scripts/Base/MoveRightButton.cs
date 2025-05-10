using System;


public class MoveRightButton : BaseButton
{
    public event Action OnMoveRightButtonClicked;

    protected override void OnClick()
    {
        OnMoveRightButtonClicked?.Invoke();
    }
}
