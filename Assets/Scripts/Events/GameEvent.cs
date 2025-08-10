
public static class GameEvent
{
    public const string PLAYER_DEAD = "PLAYER_DEAD";
    public const string HEALTH_CHANGED = "HEALTH_CHANGED";
    // Level
    public const string LEVEL_COMPLETE = "LEVEL_COMPLETE";
    public const string LEVEL_FAILED = "LEVEL_FAILED";
    // Game
    public const string GAME_COMPLETE = "GAME_COMPLETE";
    public const string TIMER_COMPLETE = "TIMER_COMPLETE";
    // Items
    public const string ITEM_COLLECTED = "ITEM_COLLECTED"; // payload: (ItemType type, int amount)
    // Tasks/Missions
    public const string TASK_PROGRESS = "TASK_PROGRESS";       // payload: (string title, string progressText)
    public const string TASK_READY_FOR_DELIVERY = "TASK_READY_FOR_DELIVERY"; // payload: (string title)
    public const string TASK_COMPLETED = "TASK_COMPLETED";     // payload: (string title)

}