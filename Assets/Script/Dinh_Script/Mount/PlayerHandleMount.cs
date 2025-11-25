using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHandleMount : MonoBehaviour
{
    public static PlayerHandleMount Instance { get; private set; }
    [SerializeField] private Transform mountVisualContainer; // Nơi chứa hình ảnh của mount
    [SerializeField] private Collider2D playerCollider; // Collider của người chơi

    [SerializeField] private Player playerMovement;

    private MountSO currentMountData;
    private GameObject currentVisualObject;
    private Mount mountInWorld;

    private bool isMounted = false;
    private int playerLayer;
    private int waterLayer;

    void Awake()
    { 
        Instance = this;
        playerLayer = gameObject.layer;
        waterLayer = LayerMask.NameToLayer("Water");
    }
    void Update()
    {
        //Bấm F để xuống khỏi vật cưởi
        if(isMounted && Input.GetKeyDown(KeyCode.F))
        {
            Dismount();
        }
    }

    public void Mount(Mount worldMount,MountSO data)
    {
        if (!CanMountHere(data.type)) 
        { 
            Debug.Log("Cannot mount here!");
            return;
        }

        isMounted = true;
        currentMountData = data;

        //Ẩn Mount trong thế giới
        mountInWorld = worldMount;
        mountInWorld.gameObject.SetActive(false);

        //Hiện hình ảnh mount lên người chơi
        if(data.mountIcon != null)
        {
            currentVisualObject = Instantiate(data.mountIcon, mountVisualContainer);
            currentVisualObject.transform.localPosition = Vector3.zero;
        }
        //Cập nhật tốc độ di chuyển
        UpdatePhysiscsCollider(data.type);
        //Player.Instance.SetSpeed(data.speedMultiplier);
    }
    public void Dismount()
    {
        if (!isMounted) return;

        //Kiểm tra vị trí có thể xuống(thuyền)
        if (currentMountData.type == MountType.Water && IsTouchingLayer(waterLayer)) 
        {
            Debug.Log("Cannot dismount here!");
            return;
        }

        //Ẩn mount trên người chơi
        if (currentVisualObject != null) Destroy(currentVisualObject);

        //Hiện mount trong thế giới
        if (mountInWorld != null)
        {
            //Cập nhật vị trí mount trong thế giới
            mountInWorld.transform.position = transform.position;
            mountInWorld.gameObject.SetActive(true);
            mountInWorld = null;
        }

        //Cập nhật trạng thái
        isMounted = false;
        currentMountData = null;
        Physics2D.IgnoreLayerCollision(playerLayer, waterLayer, false);
        //Player.Instance.SetSpeed(1f);

        Debug.Log("Dismounted");
    }
    public void UpdatePhysiscsCollider(MountType type)
    {
        switch(type)
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
        if (type == MountType.Water && !isTouchingWater) return false;
        return true;
    }
    public bool IsTouchingLayer(int layerIndex)
    {
        if (playerCollider == null) return false;
        return playerCollider.IsTouchingLayers(1 << layerIndex);
    }
    public bool IsMounted() => isMounted;
}
