
namespace DudeRescueSquad.Core.LevelManagement
{
    /// <summary>
    /// The possible game level related events
    /// </summary>
    public enum GameLevelEventType
    {
        LevelLoaded,
        LevelFinished,
        PlayerDead,             // should it be a player event type?
        PlayerLevelUp,          // should it be a player event type?
        StartPlayerAttack,
        StopPlayerAttack,
        StartPlayerDash,
    }
}