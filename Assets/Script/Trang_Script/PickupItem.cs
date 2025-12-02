using Management;
using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{
    public ItemScriptableObject itemData;
    public int quantity = 1;
    public AudioClip pickupSound;
    private AudioSource audioSource;
    private bool playerInside = false;
    private float autoPickDelay = 0.1f;   

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (GetComponent<SpriteRenderer>() != null && itemData != null)
        {
            GetComponent<SpriteRenderer>().sprite = itemData.icon;
        }
        StartCoroutine(PlayDropAnimation());
    }
    private IEnumerator PlayDropAnimation()
    {
        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + new Vector3(0, 0.3f, 0);  // bay lên 0.3
        float duration = 0.15f;

        // ? Bay lên
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, peakPos, t);
            yield return null;
        }

        // ? R?i xu?ng
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(peakPos, startPos, t);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            TryPickup();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void TryPickup()
    {
        if (!playerInside) return;

        bool added = InventoryManager.Instance.AddItem(itemData, quantity);

        if (added)
        {
            // Phát âm thanh nh?t ??
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);
            }

            Destroy(gameObject);
        }
    }

}
