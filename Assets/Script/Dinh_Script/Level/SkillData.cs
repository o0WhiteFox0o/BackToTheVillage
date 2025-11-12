using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.Localization;

// Định nghĩa các loại kỹ năng bạn có trong game
public enum SkillType
{
    Farming,
    Fishing,
    Cooking,
    Foraging
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Player/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public SkillType skillType;
    public string skillName;
    public Sprite icon;

    [Header("Mốc XP Lên Cấp")]
    [Tooltip("Tổng XP cần có. Ví dụ: Cấp 2 = 100, Cấp 3 = 300...")]
    public int[] xpThresholds = new int[9]; // 9 mốc cho 10 cấp độ

    [Header("Phần thưởng Công thức (Recipe)")]
    [Tooltip("Danh sách công thức mở khóa theo từng cấp")]
    public List<RecipeReward> recipeRewards;

    [Header("Phần thưởng Lựa chọn (Profession)")]
    [Tooltip("Lựa chọn chuyên môn ở các mốc Cấp 5, 10")]
    public List<ProfessionChoice> professionChoices;
}

// === CÁC CLASS HỖ TRỢ (Để bên ngoài class SkillData) ===

[System.Serializable]
public class RecipeReward
{
    [Tooltip("Công thức sẽ được mở khóa khi đạt cấp này")]
    public int levelToUnlock;
    public CraftingRecipeSO recipeToUnlock;
}

[System.Serializable]
public class ProfessionChoice
{
    [Tooltip("Lựa chọn sẽ xuất hiện khi đạt cấp này (ví dụ: 5 hoặc 10)")]
    public int levelToUnlock;
    public LocalizedString choiceTitle; // Ví dụ: "Chuyên gia Câu cá"
    [TextArea] public LocalizedString choiceDescription;
    public PerkSO perkA; // Lựa chọn A (Chúng ta sẽ tạo script này ở dưới)
    public PerkSO perkB; // Lựa chọn B
}