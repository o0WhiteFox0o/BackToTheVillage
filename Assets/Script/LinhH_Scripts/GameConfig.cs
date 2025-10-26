using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameConfig
{
    public List<HotkeyConfig> hotkeys;


    /// <summary>
    /// Lấy các thông số game config từ file json.
    /// </summary>
    public static GameConfig LoadGameConfig()
    {
        string gameConfigPath = Path.Combine(Application.streamingAssetsPath, GameConstants.GAME_CONFIG_PATH);

        // nếu file không tồn tại thì trả về giá trị null
        if (!File.Exists(gameConfigPath))
        {
            Debug.LogError("Không tìm tháy file GameConfig");
            return null;
        }

        // đọc dữ liệu trong file
        string fileContent = File.ReadAllText(gameConfigPath);
        // chuyển dữ liệu về dạng object
        GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(fileContent);

        return gameConfig;
    }
}


[Serializable]
public class HotkeyConfig
{
    public string action;
    public string keyCode;
}
