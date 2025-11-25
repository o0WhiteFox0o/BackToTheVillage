using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mount : MonoBehaviour
{
    [SerializeField] private MountSO mountData;

    private bool playerInRanged = false;
    private PlayerHandleMount playerHandler;

    void Update()
    {
        if(playerInRanged && Input.GetKeyDown(KeyCode.F))
        {
            if (playerHandler != null && !playerHandler.IsMounted())
            {
                playerHandler.Mount(this, mountData);
            }
        }
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
