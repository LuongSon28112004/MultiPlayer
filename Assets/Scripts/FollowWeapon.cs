using UnityEngine;

public class FollowWeapon : MonoBehaviour
{
    [SerializeField] private Transform weaponTransform; // Gán sword vào đây
    [SerializeField] private float followSpeed = 8.0f;

    void Start()
    {
        weaponTransform = GameObject.FindGameObjectWithTag("weapon").transform;
    }

    void Update()
    {
        if (weaponTransform != null)
        {
            // Cập nhật vị trí của nhân vật theo sword
            transform.position = Vector2.Lerp(transform.position, weaponTransform.position, followSpeed * Time.deltaTime);
        }
    }
}
