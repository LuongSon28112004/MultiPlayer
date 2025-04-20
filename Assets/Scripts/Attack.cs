using UnityEngine;

public class Attack : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed = 8f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}
