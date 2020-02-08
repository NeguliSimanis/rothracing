public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

public enum GameState
{
    MainMenu,
    GameOver,
    StartingRace,
    Racing
}

public enum EnemyState
{
    Dying,
    ChasingPlayer,
    Patrolling,
    AttackingPlayer,
    AttackingCooldown
}

public enum PickupType
{
    Rum,
    Ammo,
    Life
}