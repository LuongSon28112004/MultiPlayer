using Unity.Netcode;
using UnityEngine;

public class MoveRightButton : BaseButton
{
    [SerializeField] private GameObject player;
    protected override void OnClick()
    {
        player.transform.Translate(Vector3.right * 5.0f * Time.deltaTime);
    }
}
