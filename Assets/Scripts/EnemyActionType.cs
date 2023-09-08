/// <summary>
/// An action type that tells the reason why the enemy is currently moving
/// </summary>
public enum EnemyActionType
{
    /// <summary>The purpose of enemy's movement is currently unknown or not set, yet</summary>
    None,
    /// <summary>The enemy have successfully found a target and is now moving to it</summary>
    Traveling,
    /// <summary>The target is unreachable and the enemy is roaming around</summary>
    Roaming
}
