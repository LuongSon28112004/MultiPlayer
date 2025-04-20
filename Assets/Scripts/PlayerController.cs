
using UnityEngine;
using Unity.Netcode;
using System;

public struct AnimationState : INetworkSerializable
{
    public bool isRunning;
    // public bool isJumping;
    // public bool isFalling;
    // public bool isAttacking;
    public bool isLeft;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isRunning);
        // serializer.SerializeValue(ref isJumping);
        // serializer.SerializeValue(ref isFalling);
        // serializer.SerializeValue(ref isAttacking);
        serializer.SerializeValue(ref isLeft);
    }
}

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    private float speed = 5.0f;
    private float speedJump = 7.0f;


    // Biến NetworkVariable chứa toàn bộ trạng thái animation
    private NetworkVariable<AnimationState> networkedAnimationState = new NetworkVariable<AnimationState>(
        new AnimationState(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> networkedPosition = new NetworkVariable<Vector3>(
    Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsOwner) // Chỉ client sở hữu mới xử lý input
        {
            Movement();
            if (IsServer || IsHost)
            {
                // Cập nhật trạng thái animation lên server nếu là host
                networkedAnimationState.Value = GetCurrentAnimationState();
                networkedPosition.Value = transform.position; // Cập nhật vị trí nếu là server
            }
            else
            {
                // Nếu là client, gửi yêu cầu cập nhật lên server
                UpdateStateServerRpc(GetCurrentAnimationState(),transform.position);
            }
        }
        else
        {
            // Client khác lấy trạng thái từ server và cập nhật animation
            ApplyAnimationState(networkedAnimationState.Value);
            transform.position = networkedPosition.Value; // Lấy vị trí từ server
        }
    }

    private void Movement()
    {
        move();
        jumping();
    }

    private void move()
    {
        AnimationState state = GetCurrentAnimationState();
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);


        if (movement.x > 0)
        {
            state.isRunning = true;
            state.isLeft = false;
        }
        else if (movement.x < 0)
        {
            state.isRunning = true;
            state.isLeft = true;
        }
        else
        {
            state.isRunning = false;
        }

        spriteRenderer.flipX = state.isLeft;
        transform.Translate(movement * speed * Time.deltaTime);
        ApplyAnimationState(state);
    }



    private void jumping()
    {
        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector2.up * speedJump, ForceMode2D.Impulse);
        }
    }

    private AnimationState GetCurrentAnimationState()
    {
        return new AnimationState
        {
            isRunning = anim.GetBool("IsRunning"),
            // isJumping = anim.GetBool("IsJumping"),
            // isFalling = anim.GetBool("IsFalling"),
            // isAttacking = anim.GetBool("IsAttacking"),
            isLeft = spriteRenderer.flipX
        };
    }

    private void ApplyAnimationState(AnimationState state)
    {
        anim.SetBool("IsRunning", state.isRunning);
        // anim.SetBool("IsJumping", state.isJumping);
        // anim.SetBool("IsFalling", state.isFalling);
        // anim.SetBool("IsAttacking", state.isAttacking);
        spriteRenderer.flipX = state.isLeft;
    }

    [Rpc(SendTo.Server)]
    private void UpdateStateServerRpc(AnimationState newState,Vector3 newPosition)
    {
        networkedAnimationState.Value = newState;
        networkedPosition.Value = newPosition;
    }
}

