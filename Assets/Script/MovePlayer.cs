using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    [Header("References")]
    public TileCursorFollow tileCursorFollow;

    private Vector2 movement;
    private Vector2 lastDirection;
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
        if (!animator) animator = GetComponent<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isUsingTool) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("IsMoving", movement != Vector2.zero);

        if (movement != Vector2.zero)
        {
            lastDirection = ConvertTo4Diagonal(movement);
            animator.SetFloat("X", lastDirection.x);
            animator.SetFloat("Y", lastDirection.y);
        }
    }

    private void FixedUpdate()
    {
        if (!isUsingTool)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // ====================================
    // ÉP Input → 4 hướng XÉO isometric
    // ====================================
    private Vector2 ConvertTo4Diagonal(Vector2 input)
    {
        // hướng chéo sẽ dựa vào input.x và input.y
        float x = Mathf.Sign(input.x);
        float y = Mathf.Sign(input.y);

        // Nếu người chơi chỉ nhấn lên/xuống hoặc trái/phải → vẫn ép sang hướng chéo
        if (input.x == 0 && input.y > 0) return new Vector2(1, 1);      // Up (ép x = 1)
        if (input.x == 0 && input.y < 0) return new Vector2(-1, -1);    // Down
        if (input.y == 0 && input.x > 0) return new Vector2(1, -1);     // Right
        if (input.y == 0 && input.x < 0) return new Vector2(-1, 1);     // Left

        // nếu có cả X và Y → giữ nguyên hướng chéo
        return new Vector2(x, y);
    }

    // ====================================
    // TOOL theo vị trí TILE — cũng 4 hướng XÉO
    // ====================================
    private void PlayToolAnimation(string triggerName)
    {
        if (!tileCursorFollow || !tileCursorFollow.cursorObject) return;

        isUsingTool = true;

        Vector2 dir = tileCursorFollow.cursorObject.position - transform.position;

        lastDirection = ConvertTo4Diagonal(dir);

        animator.SetFloat("X", lastDirection.x);
        animator.SetFloat("Y", lastDirection.y);

        animator.SetBool("IsMoving", false);
        animator.SetTrigger(triggerName);

        StartCoroutine(ToolCooldown(0.5f));
    }

    private IEnumerator ToolCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isUsingTool = false;
    }
}
