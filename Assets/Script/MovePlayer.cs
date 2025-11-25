using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    // <--- Biến lưu hệ số nhân tốc độ (Mặc định là 1 = 100%)
    private float currentSpeedMultiplier = 1f;

    [Header("References")]
    public TileCursorFollow tileCursorFollow;

    private Vector2 movement;
    private Vector2 lastDirection;
    private bool isUsingTool = false;

    private void Awake()
    {
        Instance = this;
    }
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
            //Nhân thêm currentSpeedMultiplier vào công thức
            float finalSpeed = moveSpeed * currentSpeedMultiplier;
            rb.MovePosition(rb.position + movement.normalized * finalSpeed * Time.fixedDeltaTime);
        }
    }

    //Hàm công khai để PlayerMountHandler gọi vào
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        // Debug.Log($"[Player] Đã thay đổi tốc độ. Hệ số: {currentSpeedMultiplier}");
    }

    private Vector2 ConvertTo4Diagonal(Vector2 input)
    {
        float x = Mathf.Sign(input.x);
        float y = Mathf.Sign(input.y);

        if (input.x == 0 && input.y > 0) return new Vector2(1, 1);
        if (input.x == 0 && input.y < 0) return new Vector2(-1, -1);
        if (input.y == 0 && input.x > 0) return new Vector2(1, -1);
        if (input.y == 0 && input.x < 0) return new Vector2(-1, 1);

        return new Vector2(x, y);
    }

    // ====================================
    // TOOL theo vị trí TILE
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

        StartCoroutine(ToolCooldown(1f));
    }

    private IEnumerator ToolCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isUsingTool = false;
    }
}