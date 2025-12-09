// 
// Member: LinhH
// Date: 10/11/2025
// 


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Dùng để lưu và load các thông tin của màn chơi được lưu.
/// </summary>
public class SavedGameConfig
{
    public string farmName;
    public string characterName;
    public float playedTime;
    public float currency;

    public List<QuestData> activeQuest_List = new List<QuestData>();


    /// <summary>
    /// Load và trả về Saved Game config dựa vào farm name.
    /// </summary>
    public static SavedGameConfig LoadSaveGameByName(string farmName)
    {
        // lấy vị trí của file saved game config
        var savedGameFile = $"{GameConstants.SAVED_GAMES_FOLDER}/{farmName}.json";
        string savedGame = Path.Combine(Application.streamingAssetsPath, savedGameFile);

        return LoadSavedGameByPath(savedGame);
    }


    /// <summary>
    /// Load và trả về Saved Game config dựa vào file path.
    /// </summary>
    public static SavedGameConfig LoadSavedGameByPath(string path)
    {
        // nếu file không tồn tại thì trả về giá trị null
        if (!File.Exists(path))
        {
            Debug.LogError("Không tìm tháy file Saved Farm Config");
            return null;
        }

        // đọc dữ liệu trong file
        string fileContent = File.ReadAllText(path);

        // chuyển dữ liệu về dạng object
        SavedGameConfig savedGameConfig = JsonUtility.FromJson<SavedGameConfig>(fileContent);

        return savedGameConfig;
    }


    public void SaveGameConfig(string farmName)
    {
        // chuyển dữ liệu saved game config sang dạng json
        string savedData = JsonUtility.ToJson(this, true);

        // kiểm tra thư mục saved game đã tồn tại chưa, nếu chưa thì tạo thư mục mới
        string savedFolderPath = Path.Combine(Application.streamingAssetsPath, GameConstants.SAVED_GAMES_FOLDER);
        if (!Directory.Exists(savedFolderPath))
        {
            Directory.CreateDirectory(savedFolderPath);
        }

        // lưu dữ liệu farm vào file saved game 
        string savedFilePath = Path.Combine(savedFolderPath, $"{farmName}.json");
        File.WriteAllText(savedFilePath, savedData);
    }
}


/// <summary>
/// Lưu trữ tiến trình của một nhiệm vụ.
/// </summary>
[Serializable]
public class QuestData
{
    public string questId;
    public int questType;
    public string questJsonData;
}


/// <summary>
/// Lưu trữ tiến trình của nhiệm vụ thu thập.
/// </summary>
[Serializable]
public class CollectionQuestData
{
    public List<ItemCollectedData> collectedItem_List = new List<ItemCollectedData>();
}


/// <summary>
/// Lưu trữ tiến trình thu thập của một vật phẩm trong nhiệm vụ thu thập.
/// </summary>
[Serializable]
public class ItemCollectedData
{
    public string itemId;
    public int currentQuantity;
    public int totalQuantity;
}


public enum CharacterCurrentArea
{
    MainArea,
    MarketArea,
    LakeArea,
    Area1,
    Area2,
    Area3,
    Area4
}