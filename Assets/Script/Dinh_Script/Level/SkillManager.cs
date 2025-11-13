using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    [Header("Danh sách Kỹ năng")]
    [SerializeField] private List<SkillData> allSkillsData;

    // (Giữ nguyên class PlayerSkill)
    public class PlayerSkill
    {
        public SkillData Data { get; private set; }
        public int CurrentLevel { get; set; }
        public int CurrentXP { get; set; }

        public PlayerSkill(SkillData data)
        {
            Data = data;
            CurrentLevel = 1;
            CurrentXP = 0;
        }

        public int GetXPForNextLevel()
        {
            if (CurrentLevel > Data.xpThresholds.Length) return 0;
            return Data.xpThresholds[CurrentLevel - 1];
        }
    }

    private Dictionary<SkillType, PlayerSkill> playerSkills;

    public System.Action<PlayerSkill> OnSkillUpdated;
    public System.Action<PlayerSkill> OnSkillLeveledUp;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        InitializeSkills();
    }

    private void InitializeSkills()
    {
        playerSkills = new Dictionary<SkillType, PlayerSkill>();
        foreach (SkillData data in allSkillsData)
        {
            if (data != null && !playerSkills.ContainsKey(data.skillType))
            {
                playerSkills.Add(data.skillType, new PlayerSkill(data));
            }
        }
    }


    public void AddXP(SkillType type, int amount)
    {
        if (!playerSkills.ContainsKey(type))
        {
            Debug.LogWarning($"Không tìm thấy kỹ năng loại: {type}");
            return;
        }

        PlayerSkill skill = playerSkills[type];

        int maxLevel = skill.Data.xpThresholds.Length + 1;


        if (skill.CurrentLevel >= maxLevel)
        {
            // Nếu đã đạt cấp tối đa
            skill.CurrentXP = 0; // Đảm bảo XP luôn là 0
            OnSkillUpdated?.Invoke(skill); // Gửi cập nhật cuối cho UI (để hiển thị "MAX")
            return; // Không cộng XP nữa
        }

        // (Code cộng XP và CheckForLevelUp giữ nguyên)
        skill.CurrentXP += amount;
        OnSkillUpdated?.Invoke(skill);
        CheckForLevelUp(skill);
    }

    /// <summary>
    /// Kiểm tra lên cấp (ĐÃ SỬA LẠI - Bổ sung dòng gọi GrantRewardsForLevel)
    /// </summary>
    private void CheckForLevelUp(PlayerSkill skill)
    {
        int maxLevel = skill.Data.xpThresholds.Length + 1;
        int xpToNextLevel = skill.GetXPForNextLevel();

        // Vòng lặp này chạy khi bạn lên cấp
        while (xpToNextLevel > 0 && skill.CurrentXP >= xpToNextLevel)
        {
            skill.CurrentLevel++;
            skill.CurrentXP -= xpToNextLevel;

            // (Bạn đã thấy dòng log này)
            Debug.Log($"[LOG C] LÊN CẤP! {skill.Data.skillName} đạt cấp {skill.CurrentLevel}!");

            // (Sự kiện cho UI)
            OnSkillLeveledUp?.Invoke(skill); // (Hoặc OnSkillUpgraded)

            // === DÒNG CODE BẠN BỊ THIẾU LÀ DÒNG NÀY ===
            // Gọi hàm trao phần thưởng
            GrantRewardsForLevel(skill.Data, skill.CurrentLevel);
            // === KẾT THÚC DÒNG BỊ THIẾU ===

            // Kiểm tra max level
            if (skill.CurrentLevel == maxLevel)
            {
                skill.CurrentXP = 0;
                break;
            }

            xpToNextLevel = skill.GetXPForNextLevel();
        }

        OnSkillUpdated?.Invoke(skill);
    }

    /// <summary>
    /// Trao phần thưởng (Đã thêm 3 log DEBUG màu vàng)
    /// </summary>
    private void GrantRewardsForLevel(SkillData skillData, int newLevel)
    {
        //Debug.LogWarning($"[DEBUG] Đang kiểm tra SkillData tên: '{skillData.name}'.");
        //Debug.LogWarning($"[DEBUG] File này có tổng cộng {skillData.recipeRewards.Count} công thức (trong Recipe Rewards).");
        //Debug.LogWarning($"[DEBUG] File này có tổng cộng {skillData.professionChoices.Count} lựa chọn (trong Profession Choices).");

        // 1. Mở khóa Công thức
        var recipesToUnlock = skillData.recipeRewards.Where(r => r.levelToUnlock == newLevel).ToList();

        Debug.Log($"[DEBUG] Tìm thấy {recipesToUnlock.Count} công thức cho Cấp {newLevel}.");

        foreach (var reward in recipesToUnlock)
        {
            if (reward.recipeToUnlock != null)
            {
                // (Đảm bảo tên hàm này khớp 100% với CraftingManager.cs)
                Debug.Log($"[DEBUG] Đang gửi công thức: {reward.recipeToUnlock.name}");
                CraftingManager.Instance?.UnLockRecipe(reward.recipeToUnlock);
            }
        }

        // 2. Mở khóa Lựa chọn Chuyên môn
        var professionChoice = skillData.professionChoices.FirstOrDefault(c => c.levelToUnlock == newLevel);
        if (professionChoice != null)
        {
            Debug.Log($"[DEBUG] Đã tìm thấy 1 Lựa chọn Cấp {newLevel}. Đang gọi UI...");
            PerkUIManager.Instance?.ShowChoice(professionChoice);
        }
    }
}