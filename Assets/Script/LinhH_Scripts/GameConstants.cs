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

    /// <summary>
    /// Tên file lưu trữ các thông tin chung của trò chơi.
    /// </summary>
    public const string GAME_CONFIG_FILE = "GameConfig.json";

    /// <summary>
    /// Tên thư mục chứa các file lưu trữ của màn chơi.
    /// </summary>
    public const string SAVED_GAMES_FOLDER = "SavedGames";

    /// <summary>
    /// Số tiền khi của nhân vật khi mới tạo game.
    /// </summary>
    public const int NEW_GAME_CURRENCY = 1500;

    /// <summary>
    /// Vị trí ban đầu của nhân vật khi tạo game.
    /// </summary>
    public readonly static Vector3 NEW_GAME_POSITION = Vector3.zero;

    public const float FISHING_MAX_REACTION_TIME = 1.5f;
}


public enum FishingState
{
    Idle,
    Charging,
    Casting,
    BobberWaiting,
    FightingFish,
    PullingFish
}