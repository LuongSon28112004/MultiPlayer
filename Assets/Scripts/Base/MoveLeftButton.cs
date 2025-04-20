using Unity.Netcode;
using UnityEngine;

public class MoveLeftButton : BaseButton
{
    [SerializeField] private GameObject player;
    
     protected override void OnClick()
    {
        player.transform.Translate(Vector3.left * 5.0f * Time.deltaTime);
    }

}

