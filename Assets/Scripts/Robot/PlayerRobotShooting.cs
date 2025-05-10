using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRobotShooting : NetworkBehaviour
{
    int count = 0;
    private Vector3 right = new Vector3(2f, 0f, 0);
    private Vector3 left = new Vector3(-2f, 0f, 0);
    private bool isFlipX = false;

    [SerializeField] private ShootingButton shootingButton;

    public override void OnNetworkSpawn()
    {
        if (shootingButton == null)
        {
            shootingButton = FindFirstObjectByType<ShootingButton>();
        }
        shootingButton.OnShootingButtonClicked += CheckShootingMobile;
    }

    private void CheckShootingMobile()
    {
        if(!IsOwner) return;
        if (count == 0)
        {
            count = 1;
            isFlipX = GetComponent<SpriteRenderer>().flipX;
            ShootServerRpc(transform.position, transform.rotation, isFlipX);
        }
        
        StartCoroutine(ResetCount());
    }

    private IEnumerator ResetCount()
    {
        yield return new WaitForSeconds(0.3f);
        count = 0;
    }

    void Update()
    {
        // Chỉ xử lý input nếu là chủ sở hữu của object
        if (!IsOwner) return;
        CheckShooting();
    }

    // Truyền vị trí và góc quay của người chơi từ client lên server
    [Rpc(SendTo.Server)]
    private void ShootServerRpc(Vector3 spawnPos, Quaternion spawnRot,bool isDirection)
    {
        Vector3 buffer = isDirection ? left : right;
        BulletPoolManager.Instance.SpawnBullet(spawnPos + buffer, spawnRot,isDirection);
    }

    private void CheckShooting()
    {
        // Khi nhấn phím E và chưa bắn (để tránh bắn nhiều khi giữ phím)
        if (InputManager.Instance.IsKeyE && count == 0)
        {
            count = 1;
            isFlipX = GetComponent<SpriteRenderer>().flipX;
            ShootServerRpc(transform.position, transform.rotation,isFlipX);
        }
        else if (!InputManager.Instance.IsKeyE)
        {
            count = 0;
        }
    }
}
