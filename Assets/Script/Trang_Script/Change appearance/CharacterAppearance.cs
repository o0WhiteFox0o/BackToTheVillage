using UnityEngine;

public class AppearanceController : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer body;
    public SpriteRenderer face;
    public SpriteRenderer hair;

    [Header("Data")]
    public BodySetSO bodySet;
    public FaceSetSO faceSet;
    public HairSetSO hairSet;

    private Animator animator;
    private float timer;
    private int frameIndex;
    public float frameRate = 0.15f;

    private string lastState = "";     // ? thêm

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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

        // Xác ??nh state hi?n t?i
        string currentState =
            state.IsName("Digging Tree") ? "Digging" :
            state.IsName("Watering Tree") ? "Watering" :
            (isMoving ? "Walk" : "Idle");

        // --- FIX: Reset frameIndex khi state thay ??i ---
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
                GetFrame(hairSet, dir, "Digging")
            );
            return;
        }

        // WATERING
        if (currentState == "Watering")
        {
            SetSprites(
                GetFrame(bodySet, dir, "Watering"),
                GetFrame(faceSet, dir, "Watering"),
                GetFrame(hairSet, dir, "Watering")
            );
            return;
        }

        // MOVE / IDLE
        SetSprites(
            GetFrame(bodySet, dir, isMoving ? "Walk" : "Idle"),
            GetFrame(faceSet, dir, isMoving ? "Walk" : "Idle"),
            GetFrame(hairSet, dir, isMoving ? "Walk" : "Idle")
        );
    }

    private void SetSprites(Sprite bodySpr, Sprite faceSpr, Sprite hairSpr)
    {
        body.sprite = bodySpr;
        face.sprite = faceSpr;
        hair.sprite = hairSpr;
    }

    private Direction GetDirection(float x, float y)
    {
        if (x == -1 && y == 1) return Direction.LT;
        if (x == -1 && y == -1) return Direction.LD;
        if (x == 1 && y == 1) return Direction.RT;
        return Direction.RD;
    }

    private Sprite GetFrame(AppearanceSet set, Direction dir, string action)
    {
        Sprite[] frames = action switch
        {
            "Idle" => dir switch
            {
                Direction.LT => set.LT_idle,
                Direction.LD => set.LD_idle,
                Direction.RT => set.RT_idle,
                Direction.RD => set.RD_idle
            },
            "Walk" => dir switch
            {
                Direction.LT => set.LT_walk,
                Direction.LD => set.LD_walk,
                Direction.RT => set.RT_walk,
                Direction.RD => set.RD_walk
            },
            "Digging" => dir switch
            {
                Direction.LT => set.LT_digging,
                Direction.LD => set.LD_digging,
                Direction.RT => set.RT_digging,
                Direction.RD => set.RD_digging
            },
            "Watering" => dir switch
            {
                Direction.LT => set.LT_watering,
                Direction.LD => set.LD_watering,
                Direction.RT => set.RT_watering,
                Direction.RD => set.RD_watering
            },
            _ => set.LT_idle
        };

        return frames[frameIndex % frames.Length];
    }
}
