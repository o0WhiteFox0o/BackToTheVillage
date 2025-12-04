
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeBehind : MonoBehaviour
{
    public float fadeAlpha = 0.4f;
    public float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private bool playerInside = false;
    private Transform player;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null)
        {
            FadeTo(1f); // kh?ng c? player ? lu?n r?
            return;
        }

        // ?i?u ki?n m?: player ch?m + ??ng tr?n object
        if (playerInside && player.position.y > transform.position.y)
        {
            FadeTo(fadeAlpha);
        }
        else
        {
            FadeTo(1f);
        }
    }
    private void FadeTo(float targetAlpha)
{
    Color c = sr.color;
    c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
    sr.color = c;
}

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        playerInside = true;
        player = other.transform;
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        playerInside = false;
        player = null;
    }
}
}