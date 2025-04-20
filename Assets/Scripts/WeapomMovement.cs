using Unity.Netcode;
using UnityEngine;

public class WeapomMovement : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speedForce = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner) this.addForce();
    }

    private void addForce()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 forceWeapon = new Vector2(x, y);
        rb.AddForce(forceWeapon * speedForce, ForceMode2D.Impulse);
    }
}
