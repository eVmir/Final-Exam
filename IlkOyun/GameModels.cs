using Microsoft.Xna.Framework;

namespace IlkOyun;

internal enum GamePhase
{
    TitleScreen,
    Overworld,
    Battle,
    Victory,
    Defeat
}

internal enum ElementType
{
    Fire,
    Water,
    Wind
}

internal enum DoorType
{
    MiniBoss,
    FinalBoss
}

internal enum BattleVisualType
{
    None,
    AttackSpell,
    FireSpell,
    WaterSpell,
    WindSpell,
    CatchPulse,
    GuardShield
}

internal sealed class EnemyDefinition
{
    public EnemyDefinition(
        string name,
        int maxHp,
        int minDamage,
        int maxDamage,
        bool isBoss,
        string introMessage,
        string victoryMessage,
        Color primaryColor,
        Color secondaryColor,
        ElementType? rewardElement,
        bool requiresCatchForCompletion)
    {
        Name = name;
        MaxHp = maxHp;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        IsBoss = isBoss;
        IntroMessage = introMessage;
        VictoryMessage = victoryMessage;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        RewardElement = rewardElement;
        RequiresCatchForCompletion = requiresCatchForCompletion;
    }

    public string Name { get; }

    public int MaxHp { get; }

    public int MinDamage { get; }

    public int MaxDamage { get; }

    public bool IsBoss { get; }

    public string IntroMessage { get; }

    public string VictoryMessage { get; }

    public Color PrimaryColor { get; }

    public Color SecondaryColor { get; }

    public ElementType? RewardElement { get; }

    public bool RequiresCatchForCompletion { get; }
}

internal sealed class DoorDefinition
{
    public DoorDefinition(
        string name,
        Rectangle bounds,
        EnemyDefinition enemy,
        DoorType doorType,
        int requiredSwitchId,
        Color frameColor,
        string description)
    {
        Name = name;
        Bounds = bounds;
        Enemy = enemy;
        DoorType = doorType;
        RequiredSwitchId = requiredSwitchId;
        FrameColor = frameColor;
        Description = description;
    }

    public string Name { get; }

    public Rectangle Bounds { get; }

    public EnemyDefinition Enemy { get; }

    public DoorType DoorType { get; }

    public int RequiredSwitchId { get; }

    public Color FrameColor { get; }

    public string Description { get; }

    public bool Completed { get; set; }
}

internal sealed class PuzzleSwitch
{
    public PuzzleSwitch(int id, string label, Rectangle bounds, Color color)
    {
        Id = id;
        Label = label;
        Bounds = bounds;
        Color = color;
    }

    public int Id { get; }

    public string Label { get; }

    public Rectangle Bounds { get; }

    public Color Color { get; }

    public bool Activated { get; set; }
}

internal sealed class PuzzleBarrier
{
    public PuzzleBarrier(int switchId, Rectangle bounds, Color color)
    {
        SwitchId = switchId;
        Bounds = bounds;
        Color = color;
    }

    public int SwitchId { get; }

    public Rectangle Bounds { get; }

    public Color Color { get; }
}

internal sealed class PatrollingEnemy
{
    public PatrollingEnemy(Vector2 position, int width, int height, float leftLimit, float rightLimit, float speed, Color bodyColor)
    {
        Position = position;
        Width = width;
        Height = height;
        LeftLimit = leftLimit;
        RightLimit = rightLimit;
        Speed = speed;
        BodyColor = bodyColor;
    }

    public Vector2 Position { get; set; }

    public int Width { get; }

    public int Height { get; }

    public float LeftLimit { get; }

    public float RightLimit { get; }

    public float Speed { get; }

    public Color BodyColor { get; }

    public bool MoveRight { get; set; } = true;

    public bool IsAlive { get; set; } = true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);
}

internal sealed class UpgradeDrop
{
    public UpgradeDrop(Vector2 position, string label, Color color)
    {
        Position = position;
        Label = label;
        Color = color;
    }

    public Vector2 Position { get; set; }

    public string Label { get; }

    public Color Color { get; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 30, 30);
}

internal sealed class ThemePlatformPiece
{
    public ThemePlatformPiece(Rectangle bounds, Color bodyColor, Color topColor)
    {
        Bounds = bounds;
        BodyColor = bodyColor;
        TopColor = topColor;
    }

    public Rectangle Bounds { get; }

    public Color BodyColor { get; }

    public Color TopColor { get; }
}

internal sealed class HazardZone
{
    public HazardZone(Rectangle bounds, Color color, string label)
    {
        Bounds = bounds;
        Color = color;
        Label = label;
    }

    public Rectangle Bounds { get; }

    public Color Color { get; }

    public string Label { get; }
}

internal sealed class FireTurret
{
    public FireTurret(Vector2 position, bool shootLeft, float fireInterval, Color color)
    {
        Position = position;
        ShootLeft = shootLeft;
        FireInterval = fireInterval;
        Cooldown = fireInterval * 0.65f;
        Color = color;
    }

    public Vector2 Position { get; set; }

    public bool ShootLeft { get; }

    public float FireInterval { get; }

    public float Cooldown { get; set; }

    public Color Color { get; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 30, 28);
}

internal sealed class FireProjectile
{
    public FireProjectile(Vector2 position, float velocityX, Color color)
    {
        Position = position;
        VelocityX = velocityX;
        Color = color;
    }

    public Vector2 Position { get; set; }

    public float VelocityX { get; }

    public Color Color { get; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 18, 10);
}

internal sealed class FlyingEnemy
{
    public FlyingEnemy(Vector2 position, float leftLimit, float rightLimit, float speed, float amplitude, float phase, Color bodyColor)
    {
        Position = position;
        BaseY = position.Y;
        LeftLimit = leftLimit;
        RightLimit = rightLimit;
        Speed = speed;
        Amplitude = amplitude;
        Phase = phase;
        BodyColor = bodyColor;
    }

    public Vector2 Position { get; set; }

    public float BaseY { get; }

    public float LeftLimit { get; }

    public float RightLimit { get; }

    public float Speed { get; }

    public float Amplitude { get; }

    public float Phase { get; }

    public Color BodyColor { get; }

    public bool MoveRight { get; set; } = true;

    public bool IsAlive { get; set; } = true;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 42, 34);
}

internal sealed class WaterZone
{
    public WaterZone(Rectangle bounds, Color color)
    {
        Bounds = bounds;
        Color = color;
    }

    public Rectangle Bounds { get; }

    public Color Color { get; }
}

internal sealed class BubbleLiftZone
{
    public BubbleLiftZone(Rectangle bounds, Color color, float liftVelocity)
    {
        Bounds = bounds;
        Color = color;
        LiftVelocity = liftVelocity;
    }

    public Rectangle Bounds { get; }

    public Color Color { get; }

    public float LiftVelocity { get; }
}

internal sealed class DashFish
{
    public DashFish(Vector2 position, float leftLimit, float rightLimit, float patrolSpeed, float dashSpeed, Color bodyColor)
    {
        Position = position;
        HomePosition = position;
        LeftLimit = leftLimit;
        RightLimit = rightLimit;
        PatrolSpeed = patrolSpeed;
        DashSpeed = dashSpeed;
        Cooldown = 1.1f;
        BodyColor = bodyColor;
    }

    public Vector2 Position { get; set; }

    public Vector2 HomePosition { get; }

    public float LeftLimit { get; }

    public float RightLimit { get; }

    public float PatrolSpeed { get; }

    public float DashSpeed { get; }

    public float Cooldown { get; set; }

    public float DashDirection { get; set; } = 1f;

    public bool IsDashing { get; set; }

    public bool MoveRight { get; set; } = true;

    public bool IsAlive { get; set; } = true;

    public Color BodyColor { get; }

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 36, 24);
}

internal sealed class WorldThemePalette
{
    public WorldThemePalette(string name, Color skyTop, Color skyMiddle, Color horizon, Color platformTop, Color glow, Color uiAccent)
    {
        Name = name;
        SkyTop = skyTop;
        SkyMiddle = skyMiddle;
        Horizon = horizon;
        PlatformTop = platformTop;
        Glow = glow;
        UiAccent = uiAccent;
    }

    public string Name { get; }

    public Color SkyTop { get; }

    public Color SkyMiddle { get; }

    public Color Horizon { get; }

    public Color PlatformTop { get; }

    public Color Glow { get; }

    public Color UiAccent { get; }
}

internal sealed class BattleState
{
    public EnemyDefinition Enemy { get; set; } = null!;

    public int PlayerHp { get; set; }

    public int PlayerMaxHp { get; set; }

    public int EnemyHp { get; set; }

    public int EnemyMaxHp { get; set; }

    public int CapturedElementCount { get; set; }

    public bool IsGuarding { get; set; }

    public bool FireAvailable { get; set; }

    public bool WaterAvailable { get; set; }

    public bool WindAvailable { get; set; }

    public float CatchBonus { get; set; }

    public BattleVisualType ActiveVisual { get; set; }

    public float VisualTimer { get; set; }

    public float GuardVisualTimer { get; set; }

    public float EnemyImpactTimer { get; set; }

    public float PlayerImpactTimer { get; set; }

    public string Message { get; set; } = string.Empty;
}
