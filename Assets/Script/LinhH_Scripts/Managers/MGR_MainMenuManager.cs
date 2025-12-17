// 
// Member: LinhH
// Date: 17/12/2025
// 


using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MGR_MainMenuManager : MonoBehaviour
{
    public void DeleteSavedGame(SavedGameConfig savedGame)
    {
        // load file saved game config dự vào tên nông trại
        string fileName = $"{GameConstants.SAVED_GAMES_FOLDER}/{savedGame.farmName}.json";
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path)) { File.Delete(path); }

        string metaFile = fileName + ".meta";
        path = Path.Combine(Application.streamingAssetsPath, metaFile);
        if (File.Exists(path)) { File.Delete(path); }

        // LoadSavedFarmList();
    }


    public void LoadGame(SavedGameConfig savedGame)
    {
        SceneManager.LoadScene(1);
    }


    public void StartNewGame(string farmName, string characterName)
    {
        SavedGameConfig newGame = new SavedGameConfig();

        newGame.farmName = farmName;
        newGame.characterName = characterName;
        newGame.playedTime = 0f;
        newGame.currency = GameConstants.NEW_GAME_CURRENCY;

        newGame.characterPosition = GameConstants.NEW_GAME_POSITION;

        // TODO: thiết lập ngoại hình ban đầu nhân vật

        LoadGame(newGame);
    }
}
