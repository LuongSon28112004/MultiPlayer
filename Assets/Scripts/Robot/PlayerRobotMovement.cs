using Unity.Netcode;
using UnityEngine;

public class PlayerRobotMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    NetworkVariable<bool> flipXVariable = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );
    private float jumpSpeed = 7.0f;
    private float moveSpeed = 5.0f;
    [SerializeField] private int totalStepJump = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.addOnChangeVariable();
    }

    void Update()
    {
        if (IsOwner)
        {
            this.moveHorizontal();
            this.moveVertical();
            this.flipX();
        }
    }

    private void addOnChangeVariable()
    {
        flipXVariable.OnValueChanged += (oldValue, newValue) =>
        {
            if (!IsOwner)
            {
                spriteRenderer.flipX = newValue;
            }
        };
    }

    private void moveHorizontal()
    {
        rb.linearVelocity = new Vector2(InputManager.Instance.HorizonrtalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void flipX()
    {
        if (InputManager.Instance.HorizonrtalInput == 0) return;
        bool value = InputManager.Instance.HorizonrtalInput < 0f ? true : false;
        spriteRenderer.flipX = value;
        if (IsHost || IsServer)
        {
            flipXVariable.Value = value;
        }
        else ChangeFlipXSeverRPC(value);

    }

    private void moveVertical()
    {
        if (InputManager.Instance.IsSpace && totalStepJump < 2)
        {
            totalStepJump++;
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
            Debug.Log("totalStepJump: " + totalStepJump);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Kiểm tra va chạm với nền đất
        {
            totalStepJump = 0;
        }
    } 

    [Rpc(SendTo.Server)]
    private void ChangeFlipXSeverRPC(bool value)
    {
        flipXVariable.Value = value;
    }
}
