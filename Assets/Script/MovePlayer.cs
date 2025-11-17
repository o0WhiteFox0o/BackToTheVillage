using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    [Header("References")]
    public TileCursorFollow tileCursorFollow; // Con trỏ để xác định hướng tool

    private Vector2 movement;
    private Vector2 lastDirection; // hướng cuối cùng player nhìn
    private bool isUsingTool = false;

    private void OnEnable()
    {
        SoilInteraction.OnToolUse += PlayToolAnimation;
    }

    private void OnDisable()
    {
        SoilInteraction.OnToolUse -= PlayToolAnimation;
    }

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isUsingTool)
        {
            // Lấy input di chuyển
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            animator.SetBool("IsMoving", movement != Vector2.zero);

            if (movement != Vector2.zero)
            {
                lastDirection = movement.normalized;
                SetAnimatorDirection(lastDirection);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isUsingTool)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Set hướng animator theo 8 hướng, dùng cho isometric
    /// </summary>
    private void SetAnimatorDirection(Vector2 dir)
    {
        float x = 0f;
        float y = 0f;

        // Chia 8 hướng cố định (-1,0,1)
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            x = Mathf.Sign(dir.x);
            y = Mathf.Sign(dir.y);
        }
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            y = Mathf.Sign(dir.y);
            x = Mathf.Sign(dir.x);
        }
        else
        {
            x = Mathf.Sign(dir.x);
            y = Mathf.Sign(dir.y);
        }

        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }

    /// <summary>
    /// Khi dùng tool, chỉ update hướng 1 lần theo con trỏ isometric
    /// </summary>
    private void PlayToolAnimation(string triggerName)
    {
        if (tileCursorFollow == null || tileCursorFollow.cursorObject == null) return;

        isUsingTool = true;

        // Lấy vector từ player tới con trỏ
        Vector2 dirToCursor = tileCursorFollow.cursorObject.position - transform.position;

        // Chuyển sang hệ trục isometric
        Vector2 isoDir;
        isoDir.x = dirToCursor.x + dirToCursor.y;
        isoDir.y = dirToCursor.y - dirToCursor.x;
        isoDir.Normalize();

        if (isoDir != Vector2.zero)
            lastDirection = isoDir;

        SetAnimatorDirection(lastDirection);

        animator.SetTrigger(triggerName);
        animator.SetBool("IsMoving", false);

        // Thời gian animation tool (~0.5s)
        StartCoroutine(ToolCooldown(0.5f));
    }

    private IEnumerator ToolCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isUsingTool = false;
    }
}
