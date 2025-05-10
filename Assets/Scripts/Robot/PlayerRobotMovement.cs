using Unity.Netcode;
using UnityEngine;

public class PlayerRobotMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private jumpButton jumpButton;
    [SerializeField] private FixedJoystick joystick;

    NetworkVariable<bool> flipXVariable = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );
    private float jumpSpeed = 7.0f;
    private float moveSpeed = 5.0f;
    [SerializeField] private int totalStepJump = 0;

    public override void OnNetworkSpawn()
    {
        connectButtonEvents();
    }

    private void connectButtonEvents()
    {
        if (jumpButton == null)
        {
            jumpButton = FindFirstObjectByType<jumpButton>();
        }
        if (jumpButton != null)
        {
            jumpButton.OnJumpButtonClicked += moveVerticalMobile;
        }

        if (joystick == null)
        {
            joystick = FindFirstObjectByType<FixedJoystick>();
        }
    }

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
            this.moveHorizontalMobile();
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

    private void moveHorizontalMobile()
    {
        if (InputManager.Instance.HorizonrtalInput != 0) return;
        rb.linearVelocity = new Vector2(joystick.Horizontal * moveSpeed, rb.linearVelocity.y);
    }

    private void flipX()
    {
        float inputX = Mathf.Abs(joystick.Horizontal) > 0 ? joystick.Horizontal : InputManager.Instance.HorizonrtalInput;
        if (inputX != 0)
        {
            bool value = inputX < 0f;
            spriteRenderer.flipX = value;

            if (IsHost || IsServer)
                flipXVariable.Value = value;
            else
                ChangeFlipXSeverRPC(value);
        }
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

    private void moveVerticalMobile()
    {
        if (totalStepJump < 2)
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
