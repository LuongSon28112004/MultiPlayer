using Unity.Netcode;
using UnityEngine;

public class PlayerRobotAnimation : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    NetworkVariable<bool> attackVariable = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );
    private void Start()
    {
        anim = GetComponent<Animator>();
        // Đăng ký sự kiện OnValueChanged để cập nhật animation
        attackVariable.OnValueChanged += (oldValue, newValue) =>
        {
            if (!IsOwner)  // Chỉ cập nhật animation cho client khác
            {
                anim.SetBool("IsAttacking", newValue);
            }
        };
    }

    void Update()
    {
        shooting();
    }

    private void shooting()
    {
        if (IsOwner)
        {
            bool isAttacking = InputManager.Instance.IsKeyE;
            anim.SetBool("IsAttacking", isAttacking);

            if (IsHost || IsServer)
            {
                attackVariable.Value = isAttacking;
            }
            else
            {
                updateAnimationSeverRPC(isAttacking);
            }

        }
        else
        {
            anim.SetBool("IsAttacking", attackVariable.Value);
        }
    }

    [Rpc(SendTo.Server)]
    private void updateAnimationSeverRPC(bool isAttacking)
    {
        attackVariable.Value = isAttacking;
    }
}
