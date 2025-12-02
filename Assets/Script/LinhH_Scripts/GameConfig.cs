using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

[Serializable]
public class GameConfig
{
    public string languageId;
    public List<HotkeyConfig> hotkeys;
    public float musicVolume;
    public float sfxVolume;


    /// <summary>
    /// Lấy các thông số game config từ file json.
    /// </summary>
    public static GameConfig LoadGameConfig()
    {
        string gameConfigPath = Path.Combine(Application.streamingAssetsPath, GameConstants.GAME_CONFIG_FILE);

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


    public void SaveGameConfig()
    {
        // chuyển dữ liệu đối tượng sang dạng json
        var jsonConfig = JsonUtility.ToJson(this, true);

        string gameConfigPath = Path.Combine(Application.streamingAssetsPath, GameConstants.GAME_CONFIG_FILE);

        // kiểm tra nếu file config không tồn tại thì trả về giá trị null
        if (!File.Exists(gameConfigPath))
        {
            Debug.LogError("Không tìm tháy file GameConfig");
            return;
        }

        // ghi dữ liệu vào file config
        File.WriteAllText(gameConfigPath, jsonConfig);
    }
}


[Serializable]
public class HotkeyConfig
{
    public string action;
    public string keyCode;
}
