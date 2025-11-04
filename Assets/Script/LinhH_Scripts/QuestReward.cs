using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestReward
{
    // TODO: cập nhật thêm loại kinh nghiệm khi đã phát triển
    public int experience;
    public int gold;
    public List<ItemScriptableObject> items = new List<ItemScriptableObject>();

    // TODO: thêm danh sách công thức chế tạo/nấu ăn


    /// <summary>
    /// Phát thưởng cho nhân vật.
    /// </summary>
    public void GrantReward()
    {
        Debug.Log($"Player received {gold} gold!");

        // TODO: Hook into player inventory or stats system
    }
}