using UnityEngine;

public class AppearanceController : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer body;
    public SpriteRenderer face;
    public SpriteRenderer hair;
    public SpriteRenderer cloth;   // <-- m?i thêm

    [Header("Data Sets")]
    public BodySetSO bodySet;
    public ClothSetSO clothSet;
    public FaceSetSO faceSet;
    public HairSetSO hairSet;

    private Animator animator;
    private float timer;
    private int frameIndex;
    public float frameRate = 0.15f;

    private string lastState = "";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Frame timer
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            timer = 0f;
            frameIndex++;
        }

        UpdateSprites();
    }

    private void UpdateSprites()
    {
        float x = animator.GetFloat("X");
        float y = animator.GetFloat("Y");
        bool isMoving = animator.GetBool("IsMoving");

        Direction dir = GetDirection(x, y);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        string currentState =
            state.IsName("Digging Tree") ? "Digging" :
            state.IsName("Watering Tree") ? "Watering" :
            (isMoving ? "Walk" : "Idle");

        // Reset frame khi ??i state
        if (currentState != lastState)
        {
            frameIndex = 0;
            lastState = currentState;
        }

        // DIGGING
        if (currentState == "Digging")
        {
            SetSprites(
                GetFrame(bodySet, dir, "Digging"),
                GetFrame(faceSet, dir, "Digging"),
                GetFrame(hairSet, dir, "Digging"),
                GetFrame(clothSet, dir, "Digging")
            );
            return;
        }

        // WATERING
        if (currentState == "Watering")
        {
            SetSprites(
                GetFrame(bodySet, dir, "Watering"),
                GetFrame(faceSet, dir, "Watering"),
                GetFrame(hairSet, dir, "Watering"),
                GetFrame(clothSet, dir, "Watering")
            );
            return;
        }

        // WALK / IDLE
        string action = isMoving ? "Walk" : "Idle";

        SetSprites(
            GetFrame(bodySet, dir, action),
            GetFrame(faceSet, dir, action),
            GetFrame(hairSet, dir, action),
            GetFrame(clothSet, dir, action)
        );
    }

    // ---- L?y sprite cho t?ng layer ----
    private void SetSprites(Sprite bodySpr, Sprite faceSpr, Sprite hairSpr, Sprite clothSpr)
    {
        body.sprite = bodySpr;
        face.sprite = faceSpr;
        hair.sprite = hairSpr;
        cloth.sprite = clothSpr;
    }

    // ---- Xác ??nh h??ng ----
    private Direction GetDirection(float x, float y)
    {
        if (x < 0 && y > 0) return Direction.LT;
        if (x < 0 && y < 0) return Direction.LD;
        if (x > 0 && y > 0) return Direction.RT;
        return Direction.RD;
    }

    // ---- L?y frame theo action + direction ----
    private Sprite GetFrame(AppearanceSet set, Direction dir, string action)
    {
        Sprite[] frames = action switch
        {
            "Idle" => dir switch
            {
                Direction.LT => set.LT_idle,
                Direction.LD => set.LD_idle,
                Direction.RT => set.RT_idle,
                Direction.RD => set.RD_idle,
                _ => set.RD_idle
            },

            "Walk" => dir switch
            {
                Direction.LT => set.LT_walk,
                Direction.LD => set.LD_walk,
                Direction.RT => set.RT_walk,
                Direction.RD => set.RD_walk,
                _ => set.RD_walk
            },

            "Digging" => dir switch
            {
                Direction.LT => set.LT_digging,
                Direction.LD => set.LD_digging,
                Direction.RT => set.RT_digging,
                Direction.RD => set.RD_digging,
                _ => set.RD_digging
            },

            "Watering" => dir switch
            {
                Direction.LT => set.LT_watering,
                Direction.LD => set.LD_watering,
                Direction.RT => set.RT_watering,
                Direction.RD => set.RD_watering,
                _ => set.RD_watering
            },

            _ => set.LT_idle
        };

        return frames[frameIndex % frames.Length];
    }
}
