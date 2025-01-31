namespace Creotly_Studios
{
    public enum EnvironmentGenerationProgress
    {
        Active,
        Completed
    }

    public enum MeshSockets_ID
    {
        Spine,
        LeftHand,
        RightHand
    }

    public enum GenerationMode
    {
        Initial,
        Periodic
    }

    public enum SpawnMethod
    {
        Random,
        RoundRobin
    }

    public enum GunType
    {
        Handgun,
        AssaultRifle
    }

    public enum PatrolMode
    {
        Idle,
        Walk,
    }
    
    public enum CharacterType
    {
        Enemy,
        Player
    }

    public enum TargetType
    {
        Audio,
        Visual
    }

    public enum EnemyType
    {
        Mech,
        Humanoid
    }

    public enum WeaponType
    {
        Guns,
        Melee,
        Grenade
    }

    public enum WeaponClass
    {
        Primary,
        Auxillary
    }
    
    public enum TileNeighbors
    {
        Top,
        Left,
        Right,
        Bottom
    }
}