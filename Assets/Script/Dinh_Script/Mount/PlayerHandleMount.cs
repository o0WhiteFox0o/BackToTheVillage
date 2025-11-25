using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleMount : MonoBehaviour
{
    public static PlayerHandleMount Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform mountVisualContainer;
    [SerializeField] private Collider2D playerCollider;
    
    [SerializeField] private float jumpInMountHeight = 1.5f; // Độ cao khi player nhảy lên xe
    [SerializeField] private float jumpInMountDuration = 0.5f; // Thời gian để player nhảy lên xe

    private MountSO currentMountData;
    private Mount mountInWorld;

    private bool isMounted = false;
    private bool isJumpingInMount = false; // Kiểm tra player có đang nhảy lên xe hay không
    private int playerLayer;
    private int waterLayer;

    // Biến chống spam phím (Sửa lỗi vừa lên đã xuống)
    private float mountTime;

    void Awake()
    {
        Instance = this;
        playerLayer = gameObject.layer;
        waterLayer = LayerMask.NameToLayer("Water");
    }

    void Update()
    {
        // Chặn input khi đang nhảy lên xe
        if (isJumpingInMount) return;

        // Bấm F để xuống xe
        // Phải ngồi trên xe ít nhất 0.2s mới được xuống (Time.time > mountTime + 0.2f)
        if (isMounted && Input.GetKeyDown(KeyCode.F))
        {
            if (Time.time > mountTime + 0.2f) //Chống spam phím
            {
                Dismount();
            }
        }
    }

    public void Mount(Mount worldMount, MountSO data)
    {
        if (!CanMountHere(data.type))
        {
            Debug.Log("Cannot mount here!");
            return;
        }
        StartCoroutine(JumpInMount(worldMount, data));
    }

    private IEnumerator JumpInMount(Mount worldMount, MountSO data)
    {
        isJumpingInMount = true; // khóa input

        Vector3 startPos = transform.position;
        // Lấy vị trí của xe
        Vector3 targetPos = worldMount.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < jumpInMountDuration)
        {
            float t = elapsedTime / jumpInMountDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            float arc = jumpInMountHeight * 1 * t * (1 - t);
            currentPos.y += arc;
            transform.position = currentPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Đảm bảo vị trí cuối cùng chính xác
        transform.position = targetPos;
        isJumpingInMount = false;

        // Sau khi nhảy xong --> Cập nhật
        FinalizeMount(worldMount, data);
    }

    private void FinalizeMount(Mount worldMount, MountSO data)
    {
        isMounted = true;
        mountTime = Time.time;
        mountInWorld = worldMount;
        currentMountData = data;

        // Mount trở thành con của player
        Transform parentTarget = (mountVisualContainer != null) ? mountVisualContainer : transform;
        worldMount.BecomeMounted(parentTarget);

        //// Animation Ride
        if (Player.Instance.animator != null)
        {
            Player.Instance.animator.SetBool("IsRiding", true);
        }

        // Cập nhật trạng thái Vật lý
        UpdatePhysiscsCollider(data.type);
        Player.Instance.SetSpeedMultiplier(data.speedMultiplier);
        
    }
    public void Dismount()
    {

        if (!isMounted || isJumpingInMount) return;

        // Kiểm tra vị trí có thể xuống (thuyền)
        if (currentMountData.type == MountType.Water && IsTouchingLayer(waterLayer))
        {
            Debug.Log("Cannot dismount inside deep water!");
            return;
        }

        // Tách xe ra
        if (mountInWorld != null)
        {
            mountInWorld.BecomeUnmounted();
            mountInWorld = null;
        }

        // Đẩy Player sang bên cạnh một chút để không kẹt vào xe
        transform.position += Vector3.right * 0.5f; 

        isMounted = false;

        // Tắt Animation
        if (Player.Instance.animator != null)
        {
            Player.Instance.animator.SetBool("IsRiding", false);
        }

        // Cập nhật trạng thái Vật lý
        Physics2D.IgnoreLayerCollision(playerLayer, waterLayer, false);
        Player.Instance.SetSpeedMultiplier(1f);

        Debug.Log("Dismounted");
    }

    public void UpdatePhysiscsCollider(MountType type)
    {
        switch (type)
        {
            case MountType.Land:
                Physics2D.IgnoreLayerCollision(playerLayer, waterLayer, false);
                break;
            case MountType.Water:
                Physics2D.IgnoreLayerCollision(playerLayer, waterLayer, true);
                break;
        }
    }

    public bool CanMountHere(MountType type)
    {
        bool isTouchingWater = IsTouchingLayer(waterLayer);

        // 1. Thuyền thì phải ở gần nước (hoặc đang đứng dưới nước nếu game cho phép)
        if (type == MountType.Water && !isTouchingWater) return false;

        // 2. Ngựa (Land) thì KHÔNG được gọi khi đang đứng dưới nước
        if (type == MountType.Land && isTouchingWater) return false;

        return true;
    }

    public bool IsTouchingLayer(int layerIndex)
    {
        if (playerCollider == null) return false;
        return playerCollider.IsTouchingLayers(1 << layerIndex);
    }

    public bool IsMounted() => isMounted;
}