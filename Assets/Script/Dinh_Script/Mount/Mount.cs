using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mount : MonoBehaviour
{
    [SerializeField] private MountSO mountData;

    public Collider2D col;
    public Rigidbody2D rb;
    private bool playerInRanged = false;
    private PlayerHandleMount playerHandler;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (playerInRanged && Input.GetKeyDown(KeyCode.F))
        {
            if (playerHandler != null && !playerHandler.IsMounted())
            {
                playerHandler.Mount(this, mountData);
            }
        }
    }
    public void BecomeMounted(Transform playerTransform) 
    { 
        // Tắt vật lí
        if(col) col.enabled = false;
        if(rb) rb.simulated = true;

        // Biến mount thành con của player
        transform.SetParent(playerTransform);

        // reset vị trí
        transform.localPosition = Vector3.zero;

    }
    public void BecomeUnmounted() 
    { 
        transform.SetParent(null);

        if(col) col.enabled = true;
        if(rb) rb.simulated = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerInRanged = true;
            playerHandler = collision.GetComponent<PlayerHandleMount>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRanged = false;
            playerHandler = null;
        }
    }
}
