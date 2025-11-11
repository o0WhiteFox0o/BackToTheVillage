using System.IO;
using UnityEngine;

public class GameConstants
{
    /// <summary>
    /// Số lượng stack tối đa của item trong inventory.
    /// </summary>
    public const int ITEM_MAX_STACK_COUNT = 15;

    /// <summary>
    /// Số lượng game object con mặc định của inventory slot.
    /// </summary>
    /// Các game object con: Highlight slot
    public const int DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT = 1;


    public const string GAME_CONFIG_PATH = "GameConfig.json";
    public const string SAVED_GAMES_FOLDER = "SavedGames";
}
