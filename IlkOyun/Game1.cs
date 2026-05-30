using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IlkOyun;

public partial class Game1 : Game
{
    private const int WindowWidth = 1280;
    private const int WindowHeight = 720;
    private const int WorldWidth = 4100;
    private const int PlayerWidth = 42;
    private const int PlayerHeight = 54;
    private const float MoveSpeed = 250f;
    private const float JumpVelocity = -560f;
    private const float DoubleJumpVelocity = -530f;
    private const float Gravity = 1250f;
    private const float JumpBufferDuration = 0.16f;
    private const float CoyoteTimeDuration = 0.12f;
    private const float GuaranteedCatchThreshold = 0.12f;
    private const float CatchChancePerElement = 0.05f;
    private const float WindCatchBonus = 0.20f;
    private const float UnderwaterMoveSpeedMultiplier = 0.64f;
    private const float UnderwaterGravityMultiplier = 0.58f;
    private const float UnderwaterJumpMultiplier = 0.82f;
    private const float InvertedWaterGravityMultiplier = -0.44f;
    private const float WaterDashSpeed = 560f;
    private const float WaterDashDuration = 0.16f;
    private const float WaterDashCooldown = 0.42f;

    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new();
    private readonly List<Rectangle> _basePlatforms = [];
    private readonly List<Rectangle> _solidPlatforms = [];
    private readonly List<PuzzleSwitch> _puzzleSwitches = [];
    private readonly List<PuzzleBarrier> _puzzleBarriers = [];
    private readonly List<DoorDefinition> _doors = [];
    private readonly List<PatrollingEnemy> _mapEnemies = [];
    private readonly List<UpgradeDrop> _upgradeDrops = [];
    private readonly List<ThemePlatformPiece> _themePlatforms = [];
    private readonly List<HazardZone> _lavaPools = [];
    private readonly List<FireTurret> _fireTurrets = [];
    private readonly List<FireProjectile> _fireProjectiles = [];
    private readonly List<FlyingEnemy> _flyingEnemies = [];
    private readonly List<WaterZone> _waterZones = [];
    private readonly List<BubbleLiftZone> _bubbleLiftZones = [];
    private readonly List<DashFish> _dashFish = [];
    private readonly HashSet<ElementType> _capturedElements = [];

    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private SpriteFont _font = null!;
    private Texture2D _battleBackgroundTexture = null!;
    private Texture2D _battleMageTexture = null!;
    private Texture2D _battleGhostTexture = null!;
    private Texture2D _overworldMageTexture = null!;
    private Texture2D _roamingSpiritTexture = null!;
    private Texture2D _fireGroundEnemyTexture = null!;
    private Texture2D _waterGroundEnemyTexture = null!;
    private Texture2D _windGroundEnemyTexture = null!;
    private Texture2D _abyssGroundEnemyTexture = null!;
    private Texture2D _windWraithTexture = null!;
    private Texture2D _waterGolemTexture = null!;

    private SoundEffect _jumpSound = null!;
    private SoundEffect _dashSound = null!;
    private SoundEffect _pickupSound = null!;
    private SoundEffect _switchSound = null!;
    private SoundEffect _stompSound = null!;
    private SoundEffect _hurtSound = null!;
    private SoundEffect _battleEnterSound = null!;
    private SoundEffect _spellSound = null!;
    private SoundEffect _enemyCastSound = null!;
    private SoundEffect _catchSuccessSound = null!;
    private SoundEffect _catchFailSound = null!;
    private SoundEffect _victorySound = null!;

    private GamePhase _gamePhase = GamePhase.Overworld;
    private BattleState _battleState = null!;
    private DoorDefinition _activeDoor = null!;
    private WorldThemePalette _currentTheme = null!;

    private Vector2 _spawnPosition = new(72, 566);
    private Vector2 _playerPosition;
    private Vector2 _previousPlayerPosition;
    private Vector2 _playerVelocity;

    private float _cameraX;
    private float _animationTimer;
    private float _jumpBufferTimer;
    private float _coyoteTimeTimer;
    private float _bubbleDiveGraceTimer;
    private float _waterDashTimer;
    private float _waterDashCooldownTimer;
    private float _waterDashDirection = 1f;
    private float _spawnProtectionTimer;

    private bool _isOnGround;
    private bool _isUnderwater;
    private bool _playerFacingRight = true;
    private bool _hasDoubleJumpUpgrade;
    private bool _hasUsedDoubleJump;
    private bool _campaignCompleted;
    private bool _returnToOverworldAfterResult;
    private int _difficultyTier;
    private int _cycleNumber = 1;

    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;
    private string _statusMessage = string.Empty;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        Window.AllowUserResizing = false;
        Window.Title = "Soul Hunter - World Map Prototype";
    }

    protected override void Initialize()
    {
        BuildOverworld();
        ResetFullRun();
        _gamePhase = GamePhase.TitleScreen;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _font = Content.Load<SpriteFont>("DefaultFont");
        _battleBackgroundTexture = Content.Load<Texture2D>("Sprites/battle_arena_bg");
        _battleMageTexture = Content.Load<Texture2D>("Sprites/rabbit_mage_battle");
        _battleGhostTexture = Content.Load<Texture2D>("Sprites/ghost_enemy_base");
        _overworldMageTexture = Content.Load<Texture2D>("Sprites/rabbit_mage_overworld");
        _roamingSpiritTexture = Content.Load<Texture2D>("Sprites/roaming_spirit_overworld");
        _fireGroundEnemyTexture = Content.Load<Texture2D>("Sprites/ground_enemy_fire");
        _waterGroundEnemyTexture = Content.Load<Texture2D>("Sprites/ground_enemy_water");
        _windGroundEnemyTexture = Content.Load<Texture2D>("Sprites/ground_enemy_wind");
        _abyssGroundEnemyTexture = Content.Load<Texture2D>("Sprites/ground_enemy_abyss");
        _windWraithTexture = Content.Load<Texture2D>("Sprites/wind_wraith_overworld");
        _waterGolemTexture = Content.Load<Texture2D>("Sprites/water_golem_overworld");

        _jumpSound = Content.Load<SoundEffect>("Audio/jump");
        _dashSound = Content.Load<SoundEffect>("Audio/dash");
        _pickupSound = Content.Load<SoundEffect>("Audio/pickup");
        _switchSound = Content.Load<SoundEffect>("Audio/switch");
        _stompSound = Content.Load<SoundEffect>("Audio/stomp");
        _hurtSound = Content.Load<SoundEffect>("Audio/hurt");
        _battleEnterSound = Content.Load<SoundEffect>("Audio/battle_enter");
        _spellSound = Content.Load<SoundEffect>("Audio/spell");
        _enemyCastSound = Content.Load<SoundEffect>("Audio/enemy_cast");
        _catchSuccessSound = Content.Load<SoundEffect>("Audio/catch_success");
        _catchFailSound = Content.Load<SoundEffect>("Audio/catch_fail");
        _victorySound = Content.Load<SoundEffect>("Audio/victory");
    }

    protected override void Update(GameTime gameTime)
    {
        _currentKeyboardState = Keyboard.GetState();
        _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (WasKeyPressed(Keys.Escape))
        {
            Exit();
        }

        switch (_gamePhase)
        {
            case GamePhase.TitleScreen:
                UpdateTitleScreen();
                break;
            case GamePhase.Overworld:
                UpdateOverworld(gameTime);
                break;
            case GamePhase.Battle:
                UpdateBattle(gameTime);
                break;
            case GamePhase.Victory:
            case GamePhase.Defeat:
                UpdateResultScreen();
                break;
        }

        _previousKeyboardState = _currentKeyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(GetBackgroundColor());

        _spriteBatch.Begin();

        switch (_gamePhase)
        {
            case GamePhase.TitleScreen:
                DrawTitleScreen();
                break;
            case GamePhase.Overworld:
                DrawOverworld();
                break;
            case GamePhase.Battle:
                DrawBattle();
                break;
            case GamePhase.Victory:
            case GamePhase.Defeat:
                DrawResultScreen();
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateOverworld(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _previousPlayerPosition = _playerPosition;
        _spawnProtectionTimer = Math.Max(0f, _spawnProtectionTimer - deltaTime);

        RefreshSolidPlatforms();
        UpdatePlayerMovement(deltaTime);
        UpdatePuzzleSwitches();
        UpdatePatrollingEnemies(deltaTime);
        UpdateThemeThreats(deltaTime);
        HandlePatrollingEnemyCollisions();
        HandleThemeThreatCollisions();
        UpdateUpgradeDrops();
        HandleDoorInteractions();
        UpdateCamera();

        if (_playerPosition.Y > WindowHeight + 120)
        {
            RespawnToCheckpoint("You fell from the overworld. The mage returned to the checkpoint.");
        }
    }

    private void UpdateTitleScreen()
    {
        _cameraX = MathHelper.Clamp((_animationTimer * 20f) % Math.Max(1f, WorldWidth - WindowWidth), 0f, WorldWidth - WindowWidth);

        if (WasKeyPressed(Keys.Enter))
        {
            ResetOverworldState();
            _statusMessage = "The hunt begins.";
            PlaySound(_pickupSound, 0.42f, 0.08f);
        }
    }

    private void RefreshSolidPlatforms()
    {
        _solidPlatforms.Clear();

        foreach (Rectangle platform in _basePlatforms)
        {
            _solidPlatforms.Add(platform);
        }

        foreach (ThemePlatformPiece platformPiece in _themePlatforms)
        {
            _solidPlatforms.Add(platformPiece.Bounds);
        }

        foreach (PuzzleBarrier barrier in _puzzleBarriers)
        {
            if (!IsSwitchActivated(barrier.SwitchId))
            {
                _solidPlatforms.Add(barrier.Bounds);
            }
        }

        foreach (FireTurret turret in _fireTurrets)
        {
            _solidPlatforms.Add(turret.Bounds);
        }
    }

    private void UpdatePlayerMovement(float deltaTime)
    {
        _isUnderwater = IsInvertedWaterTraversal() || IsPlayerInsideWaterZone();
        _bubbleDiveGraceTimer = Math.Max(0f, _bubbleDiveGraceTimer - deltaTime);
        _waterDashTimer = Math.Max(0f, _waterDashTimer - deltaTime);
        _waterDashCooldownTimer = Math.Max(0f, _waterDashCooldownTimer - deltaTime);

        if (WasJumpPressed())
        {
            _jumpBufferTimer = JumpBufferDuration;
        }
        else
        {
            _jumpBufferTimer = Math.Max(0f, _jumpBufferTimer - deltaTime);
        }

        if (_isOnGround)
        {
            _coyoteTimeTimer = CoyoteTimeDuration;
            _hasUsedDoubleJump = false;
        }
        else
        {
            _coyoteTimeTimer = Math.Max(0f, _coyoteTimeTimer - deltaTime);
        }

        float movement = 0f;

        if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
        {
            movement -= 1f;
        }

        if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
        {
            movement += 1f;
        }

        if (movement > 0f)
        {
            _playerFacingRight = true;
        }
        else if (movement < 0f)
        {
            _playerFacingRight = false;
        }

        float moveSpeed = _isUnderwater ? MoveSpeed * UnderwaterMoveSpeedMultiplier : MoveSpeed;
        _playerVelocity.X = movement * moveSpeed;

        if (CurrentThemeIs("Water") && WasWaterDashPressed() && _waterDashCooldownTimer <= 0f)
        {
            StartWaterDash(movement);
        }

        if (CurrentThemeIs("Wind") && !_isOnGround && !_isUnderwater)
        {
            float crosswind = MathF.Sin((_animationTimer * 2.4f) + (_playerPosition.X * 0.0125f)) * (44f + (_difficultyTier * 6f));
            _playerVelocity.X += crosswind;
        }

        TryConsumeJumpInput();

        float gravity = IsInvertedWaterTraversal()
            ? Gravity * InvertedWaterGravityMultiplier
            : _isUnderwater
                ? Gravity * UnderwaterGravityMultiplier
                : Gravity;
        _playerVelocity.Y += gravity * deltaTime;

        ApplyBubbleLift();

        if (_waterDashTimer > 0f)
        {
            _playerVelocity.X = _waterDashDirection * WaterDashSpeed;
            _playerVelocity.Y *= 0.55f;
        }

        if (IsInvertedWaterTraversal())
        {
            _playerVelocity.Y = MathHelper.Clamp(_playerVelocity.Y, -320f, 360f);
        }
        else if (_isUnderwater)
        {
            _playerVelocity.Y = Math.Min(_playerVelocity.Y, 300f);
        }

        MovePlayerHorizontally(deltaTime);
        MovePlayerVertically(deltaTime);
        TryConsumeJumpInput();
    }

    private void StartWaterDash(float movement)
    {
        float direction = movement != 0f ? MathF.Sign(movement) : (_playerFacingRight ? 1f : -1f);
        _waterDashDirection = direction;
        _waterDashTimer = WaterDashDuration;
        _waterDashCooldownTimer = WaterDashCooldown;
        _playerVelocity.X = _waterDashDirection * WaterDashSpeed;
        _playerVelocity.Y *= 0.45f;
        _statusMessage = "Water dash activated.";
        PlaySound(_dashSound, 0.38f, 0.08f);
    }

    private void ApplyBubbleLift()
    {
        if (!CurrentThemeIs("Water"))
        {
            return;
        }

        foreach (BubbleLiftZone bubbleLiftZone in _bubbleLiftZones)
        {
            if (!PlayerBounds.Intersects(bubbleLiftZone.Bounds))
            {
                continue;
            }

            _bubbleDiveGraceTimer = 0.22f;
            _playerVelocity.Y = bubbleLiftZone.LiftVelocity >= 0f
                ? Math.Max(_playerVelocity.Y, bubbleLiftZone.LiftVelocity)
                : Math.Min(_playerVelocity.Y, bubbleLiftZone.LiftVelocity);
            break;
        }
    }

    private void MovePlayerHorizontally(float deltaTime)
    {
        _playerPosition.X += _playerVelocity.X * deltaTime;

        Rectangle playerBounds = PlayerBounds;
        foreach (Rectangle platform in _solidPlatforms)
        {
            if (!playerBounds.Intersects(platform))
            {
                continue;
            }

            if (_playerVelocity.X > 0)
            {
                _playerPosition.X = platform.Left - PlayerWidth;
                _waterDashTimer = 0f;
            }
            else if (_playerVelocity.X < 0)
            {
                _playerPosition.X = platform.Right;
                _waterDashTimer = 0f;
            }

            playerBounds = PlayerBounds;
        }

        _playerPosition.X = MathHelper.Clamp(_playerPosition.X, 0, WorldWidth - PlayerWidth);
    }

    private void MovePlayerVertically(float deltaTime)
    {
        _isOnGround = false;
        _playerPosition.Y += _playerVelocity.Y * deltaTime;
        bool invertedWater = IsInvertedWaterTraversal();

        Rectangle playerBounds = PlayerBounds;
        foreach (Rectangle platform in _solidPlatforms)
        {
            if (!playerBounds.Intersects(platform))
            {
                continue;
            }

            if (!invertedWater && _playerVelocity.Y > 0)
            {
                _playerPosition.Y = platform.Top - PlayerHeight;
                _playerVelocity.Y = 0f;
                _isOnGround = true;
                _hasUsedDoubleJump = false;
                _coyoteTimeTimer = CoyoteTimeDuration;
            }
            else if (!invertedWater && _playerVelocity.Y < 0)
            {
                _playerPosition.Y = platform.Bottom;
                _playerVelocity.Y = 0f;
            }
            else if (invertedWater && _playerVelocity.Y < 0)
            {
                _playerPosition.Y = platform.Bottom;
                _playerVelocity.Y = 0f;
                _isOnGround = true;
                _hasUsedDoubleJump = false;
                _coyoteTimeTimer = CoyoteTimeDuration;
            }
            else if (invertedWater && _playerVelocity.Y > 0)
            {
                _playerPosition.Y = platform.Top - PlayerHeight;
                _playerVelocity.Y = 0f;
            }

            playerBounds = PlayerBounds;
        }
    }

    private void TryConsumeJumpInput()
    {
        if (_jumpBufferTimer <= 0f)
        {
            return;
        }

        bool invertedWater = IsInvertedWaterTraversal();

        bool canBubbleDive = invertedWater && _bubbleDiveGraceTimer > 0f;

        if (_isOnGround || _coyoteTimeTimer > 0f || canBubbleDive)
        {
            float jumpStrength = invertedWater
                ? MathF.Abs(JumpVelocity) * UnderwaterJumpMultiplier
                : _isUnderwater
                    ? JumpVelocity * UnderwaterJumpMultiplier
                    : JumpVelocity;
            PerformJump(jumpStrength, false);
            return;
        }

        if (_hasDoubleJumpUpgrade && !_hasUsedDoubleJump && WasJumpPressed())
        {
            float jumpStrength = invertedWater
                ? MathF.Abs(DoubleJumpVelocity) * 0.90f
                : _isUnderwater
                    ? DoubleJumpVelocity * 0.90f
                    : DoubleJumpVelocity;
            PerformJump(jumpStrength, true);
        }
    }

    private void PerformJump(float jumpStrength, bool isDoubleJump)
    {
        _playerVelocity.Y = jumpStrength;
        _isOnGround = false;
        _jumpBufferTimer = 0f;
        _coyoteTimeTimer = 0f;
        float jumpPitch = isDoubleJump ? 0.18f : IsInvertedWaterTraversal() ? -0.08f : 0f;
        PlaySound(_jumpSound, 0.34f, jumpPitch);

        if (isDoubleJump)
        {
            _hasUsedDoubleJump = true;
            _statusMessage = IsInvertedWaterTraversal() ? "Double dive activated." : "Double jump activated.";
        }
    }

    private bool WasJumpPressed()
    {
        return WasKeyPressed(Keys.Space) || WasKeyPressed(Keys.W) || WasKeyPressed(Keys.Up);
    }

    private bool WasWaterDashPressed()
    {
        return WasKeyPressed(Keys.LeftShift) || WasKeyPressed(Keys.RightShift);
    }

    private void UpdatePuzzleSwitches()
    {
        foreach (PuzzleSwitch puzzleSwitch in _puzzleSwitches)
        {
            if (puzzleSwitch.Activated || !PlayerBounds.Intersects(puzzleSwitch.Bounds))
            {
                continue;
            }

            puzzleSwitch.Activated = true;
            _statusMessage = $"{puzzleSwitch.Label} activated. Its matching mini-boss gate can now be reached.";
            PlaySound(_switchSound, 0.44f, 0.02f);
        }
    }

    private void UpdateThemeThreats(float deltaTime)
    {
        UpdateFireTurrets(deltaTime);
        UpdateFlyingEnemies(deltaTime);
        UpdateDashFish(deltaTime);
    }

    private void UpdateFireTurrets(float deltaTime)
    {
        for (int index = _fireTurrets.Count - 1; index >= 0; index--)
        {
            FireTurret turret = _fireTurrets[index];
            turret.Cooldown = Math.Max(0f, turret.Cooldown - deltaTime);

            if (turret.Cooldown > 0f)
            {
                continue;
            }

            float projectileSpeed = turret.ShootLeft ? -(248f + (_difficultyTier * 22f)) : 248f + (_difficultyTier * 22f);
            float spawnX = turret.ShootLeft ? turret.Bounds.Left - 20 : turret.Bounds.Right + 2;
            float spawnY = turret.Bounds.Y + 8;
            _fireProjectiles.Add(new FireProjectile(new Vector2(spawnX, spawnY), projectileSpeed, new Color(255, 188, 118)));
            turret.Cooldown = Math.Max(0.70f, turret.FireInterval - (_difficultyTier * 0.05f));
        }

        for (int index = _fireProjectiles.Count - 1; index >= 0; index--)
        {
            FireProjectile projectile = _fireProjectiles[index];
            projectile.Position = new Vector2(projectile.Position.X + (projectile.VelocityX * deltaTime), projectile.Position.Y);

            if (projectile.Position.X < -40f || projectile.Position.X > WorldWidth + 40f)
            {
                _fireProjectiles.RemoveAt(index);
                continue;
            }

            bool hitPlatform = false;
            foreach (Rectangle platform in _solidPlatforms)
            {
                if (!projectile.Bounds.Intersects(platform))
                {
                    continue;
                }

                hitPlatform = true;
                break;
            }

            if (hitPlatform)
            {
                _fireProjectiles.RemoveAt(index);
            }
        }
    }

    private void UpdateFlyingEnemies(float deltaTime)
    {
        foreach (FlyingEnemy enemy in _flyingEnemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            float direction = enemy.MoveRight ? 1f : -1f;
            float nextX = enemy.Position.X + (direction * enemy.Speed * deltaTime);

            if (nextX <= enemy.LeftLimit)
            {
                nextX = enemy.LeftLimit;
                enemy.MoveRight = true;
            }
            else if (nextX >= enemy.RightLimit)
            {
                nextX = enemy.RightLimit;
                enemy.MoveRight = false;
            }

            float nextY = enemy.BaseY + (MathF.Sin((_animationTimer * 3.2f) + enemy.Phase) * enemy.Amplitude);
            enemy.Position = new Vector2(nextX, nextY);
        }
    }

    private void UpdateDashFish(float deltaTime)
    {
        foreach (DashFish fish in _dashFish)
        {
            if (!fish.IsAlive)
            {
                continue;
            }

            fish.Cooldown = Math.Max(0f, fish.Cooldown - deltaTime);

            if (fish.IsDashing)
            {
                float nextX = fish.Position.X + (fish.DashDirection * fish.DashSpeed * deltaTime);
                if (nextX <= fish.LeftLimit || nextX >= fish.RightLimit)
                {
                    nextX = MathHelper.Clamp(nextX, fish.LeftLimit, fish.RightLimit);
                    fish.IsDashing = false;
                    fish.Cooldown = 1.25f;
                }

                float dashY = fish.HomePosition.Y + (MathF.Sin((_animationTimer * 10f) + fish.HomePosition.X) * 5f);
                fish.Position = new Vector2(nextX, dashY);
                continue;
            }

            bool playerNear = MathF.Abs((PlayerBounds.Center.X) - (fish.Bounds.Center.X)) < 148f &&
                              MathF.Abs((PlayerBounds.Center.Y) - (fish.Bounds.Center.Y)) < 92f;

            if (playerNear && fish.Cooldown <= 0f)
            {
                fish.IsDashing = true;
                fish.DashDirection = PlayerBounds.Center.X >= fish.Bounds.Center.X ? 1f : -1f;
                fish.MoveRight = fish.DashDirection > 0f;
                continue;
            }

            float direction = fish.MoveRight ? 1f : -1f;
            float nextPatrolX = fish.Position.X + (direction * fish.PatrolSpeed * deltaTime);
            if (nextPatrolX <= fish.LeftLimit)
            {
                nextPatrolX = fish.LeftLimit;
                fish.MoveRight = true;
            }
            else if (nextPatrolX >= fish.RightLimit)
            {
                nextPatrolX = fish.RightLimit;
                fish.MoveRight = false;
            }

            float swimY = fish.HomePosition.Y + (MathF.Sin((_animationTimer * 4f) + fish.HomePosition.X) * 4f);
            fish.Position = new Vector2(nextPatrolX, swimY);
        }
    }

    private void UpdatePatrollingEnemies(float deltaTime)
    {
        foreach (PatrollingEnemy enemy in _mapEnemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            float direction = enemy.MoveRight ? 1f : -1f;
            enemy.Position = new Vector2(enemy.Position.X + (direction * enemy.Speed * deltaTime), enemy.Position.Y);

            if (enemy.Position.X <= enemy.LeftLimit)
            {
                enemy.Position = new Vector2(enemy.LeftLimit, enemy.Position.Y);
                enemy.MoveRight = true;
            }
            else if (enemy.Position.X >= enemy.RightLimit)
            {
                enemy.Position = new Vector2(enemy.RightLimit, enemy.Position.Y);
                enemy.MoveRight = false;
            }
        }
    }

    private void HandlePatrollingEnemyCollisions()
    {
        if (_spawnProtectionTimer > 0f)
        {
            return;
        }

        Rectangle playerBounds = PlayerBounds;
        Rectangle previousBounds = PreviousPlayerBounds;

        foreach (PatrollingEnemy enemy in _mapEnemies)
        {
            if (!enemy.IsAlive || !playerBounds.Intersects(enemy.Bounds))
            {
                continue;
            }

            bool stompedEnemy = previousBounds.Bottom <= enemy.Bounds.Top + 8 && _playerVelocity.Y > 0;
            if (stompedEnemy)
            {
                enemy.IsAlive = false;
                _playerVelocity.Y = JumpVelocity * 0.55f;
                _isOnGround = false;
                _statusMessage = "A roaming spirit was stomped and removed from the map.";
                PlaySound(_stompSound, 0.42f, -0.04f);

                if (!_hasDoubleJumpUpgrade && _upgradeDrops.Count == 0)
                {
                    _upgradeDrops.Add(new UpgradeDrop(new Vector2(enemy.Position.X + 4, enemy.Position.Y - 26), "DJ", new Color(255, 220, 110)));
                    _statusMessage = "Enemy dropped a Double Jump module. Collect it.";
                }

                return;
            }

            RespawnToCheckpoint("A roaming spirit touched you. The mage returned to the checkpoint.");
            return;
        }
    }

    private void HandleThemeThreatCollisions()
    {
        if (_spawnProtectionTimer > 0f)
        {
            return;
        }

        Rectangle playerBounds = PlayerBounds;
        Rectangle previousBounds = PreviousPlayerBounds;

        foreach (HazardZone lavaPool in _lavaPools)
        {
            if (!playerBounds.Intersects(lavaPool.Bounds))
            {
                continue;
            }

            RespawnToCheckpoint("The lava field burned the mage. Return to the checkpoint.");
            return;
        }

        foreach (FireTurret turret in _fireTurrets)
        {
            if (!playerBounds.Intersects(turret.Bounds))
            {
                continue;
            }

            RespawnToCheckpoint("A fire turret blasted the mage off the route.");
            return;
        }

        foreach (FireProjectile projectile in _fireProjectiles)
        {
            if (!playerBounds.Intersects(projectile.Bounds))
            {
                continue;
            }

            RespawnToCheckpoint("A fireball hit the mage. Return to the checkpoint.");
            return;
        }

        foreach (FlyingEnemy enemy in _flyingEnemies)
        {
            if (!enemy.IsAlive || !playerBounds.Intersects(enemy.Bounds))
            {
                continue;
            }

            bool stompedEnemy = previousBounds.Bottom <= enemy.Bounds.Top + 10 && _playerVelocity.Y > 0;
            if (stompedEnemy)
            {
                enemy.IsAlive = false;
                _playerVelocity.Y = JumpVelocity * 0.48f;
                _isOnGround = false;
                _statusMessage = "A flying wraith was stomped out of the air.";
                PlaySound(_stompSound, 0.40f, 0.04f);
                return;
            }

            RespawnToCheckpoint("A flying wraith knocked the mage back to the checkpoint.");
            return;
        }

        foreach (DashFish fish in _dashFish)
        {
            if (!fish.IsAlive)
            {
                continue;
            }

            if (!playerBounds.Intersects(fish.Bounds))
            {
                continue;
            }

            bool dashKilledFish = CurrentThemeIs("Water") && _waterDashTimer > 0f;
            if (dashKilledFish)
            {
                fish.IsAlive = false;
                fish.IsDashing = false;
                _waterDashTimer = 0f;
                _playerVelocity.X *= 0.35f;
                _playerVelocity.Y = -220f;
                _isOnGround = false;
                _statusMessage = "A water monster was shattered by the water dash.";
                PlaySound(_stompSound, 0.48f, -0.18f);
                return;
            }

            RespawnToCheckpoint("A water monster surged from below and knocked the mage away.");
            return;
        }
    }

    private void UpdateUpgradeDrops()
    {
        for (int index = _upgradeDrops.Count - 1; index >= 0; index--)
        {
            UpgradeDrop drop = _upgradeDrops[index];
            if (!PlayerBounds.Intersects(drop.Bounds))
            {
                continue;
            }

            _hasDoubleJumpUpgrade = true;
            _hasUsedDoubleJump = false;
            _upgradeDrops.RemoveAt(index);
            _statusMessage = "Double Jump module collected. You can now jump once more while airborne.";
            PlaySound(_pickupSound, 0.44f, 0.04f);
        }
    }

    private bool IsPlayerInsideWaterZone()
    {
        foreach (WaterZone waterZone in _waterZones)
        {
            if (PlayerBounds.Intersects(waterZone.Bounds))
            {
                return true;
            }
        }

        return false;
    }

    private void HandleDoorInteractions()
    {
        foreach (DoorDefinition door in _doors)
        {
            if (!PlayerBounds.Intersects(door.Bounds))
            {
                continue;
            }

            if (door.Completed && door.DoorType == DoorType.MiniBoss)
            {
                _statusMessage = $"{door.Name} is already cleared. Its element is stored in your Ghost Tube.";
                return;
            }

            if (!IsDoorUnlocked(door))
            {
                _statusMessage = GetDoorLockedMessage(door);
                return;
            }

            if (door.DoorType == DoorType.FinalBoss && door.Completed)
            {
                _statusMessage = "The final gate has already been conquered.";
                return;
            }

            _statusMessage = $"{door.Name} is ready. {door.Description}";

            if (WasKeyPressed(Keys.E))
            {
                StartBattle(door);
            }

            return;
        }
    }

    private void RespawnToCheckpoint(string message)
    {
        PlaySound(_hurtSound, 0.36f, -0.12f);
        _spawnPosition = GetSpawnPositionForCurrentTheme();
        _playerPosition = _spawnPosition;
        _previousPlayerPosition = _spawnPosition;
        _playerVelocity = Vector2.Zero;
        _cameraX = 0f;
        _isUnderwater = false;
        _jumpBufferTimer = 0f;
        _coyoteTimeTimer = 0f;
        _bubbleDiveGraceTimer = 0f;
        _waterDashTimer = 0f;
        _waterDashCooldownTimer = 0f;
        _waterDashDirection = 1f;
        _spawnProtectionTimer = 0.90f;
        _hasUsedDoubleJump = false;
        _statusMessage = message;
        RebuildMapEnemies();
        RebuildThemeThreats();
    }

    private void UpdateCamera()
    {
        float targetX = _playerPosition.X - (WindowWidth * 0.45f);
        _cameraX = MathHelper.Clamp(targetX, 0f, WorldWidth - WindowWidth);
    }

    private bool IsDoorUnlocked(DoorDefinition door)
    {
        if (door.DoorType == DoorType.FinalBoss)
        {
            return HasAllCapturedElements();
        }

        return IsSwitchActivated(door.RequiredSwitchId);
    }

    private bool IsSwitchActivated(int switchId)
    {
        foreach (PuzzleSwitch puzzleSwitch in _puzzleSwitches)
        {
            if (puzzleSwitch.Id == switchId)
            {
                return puzzleSwitch.Activated;
            }
        }

        return false;
    }

    private bool HasAllCapturedElements()
    {
        return _capturedElements.Count == 3;
    }

    private string GetDoorLockedMessage(DoorDefinition door)
    {
        if (door.DoorType == DoorType.FinalBoss)
        {
            return $"Final Gate is locked. Captured elements: {_capturedElements.Count} / 3.";
        }

        return $"{door.Name} is sealed. Activate its matching rune switch somewhere on the map first.";
    }

    private float GetCatchChance()
    {
        float healthRatio = _battleState.EnemyHp / (float)_battleState.EnemyMaxHp;
        if (healthRatio <= GuaranteedCatchThreshold)
        {
            return 1f;
        }

        float chance = GetHealthBasedCatchChance(healthRatio);

        chance += (_battleState.CapturedElementCount * CatchChancePerElement) + _battleState.CatchBonus;

        return MathF.Min(chance, 0.97f);
    }

    private float GetHealthBasedCatchChance(float healthRatio)
    {
        if (healthRatio <= GuaranteedCatchThreshold)
        {
            return 1f;
        }

        if (healthRatio <= 0.20f)
        {
            return 0.90f;
        }

        if (healthRatio <= 0.30f)
        {
            return 0.75f;
        }

        if (healthRatio <= 0.40f)
        {
            return 0.60f;
        }

        if (healthRatio <= 0.55f)
        {
            return 0.45f;
        }

        if (healthRatio <= 0.75f)
        {
            return 0.10f;
        }

        return 0.03f;
    }

    private void TriggerBattleVisual(BattleVisualType visualType, float duration)
    {
        _battleState.ActiveVisual = visualType;
        _battleState.VisualTimer = duration;
    }

    private void TriggerEnemyImpact(float duration)
    {
        _battleState.EnemyImpactTimer = duration;
    }

    private void TriggerPlayerImpact(float duration)
    {
        _battleState.PlayerImpactTimer = duration;
    }

    private void UpdateBattleVisualTimers(float deltaTime)
    {
        if (_battleState.VisualTimer > 0f)
        {
            _battleState.VisualTimer = Math.Max(0f, _battleState.VisualTimer - deltaTime);
            if (_battleState.VisualTimer <= 0f)
            {
                _battleState.ActiveVisual = BattleVisualType.None;
            }
        }

        if (_battleState.GuardVisualTimer > 0f)
        {
            _battleState.GuardVisualTimer = Math.Max(0f, _battleState.GuardVisualTimer - deltaTime);
        }

        if (_battleState.EnemyImpactTimer > 0f)
        {
            _battleState.EnemyImpactTimer = Math.Max(0f, _battleState.EnemyImpactTimer - deltaTime);
        }

        if (_battleState.PlayerImpactTimer > 0f)
        {
            _battleState.PlayerImpactTimer = Math.Max(0f, _battleState.PlayerImpactTimer - deltaTime);
        }
    }

    private float GetBattleVisualDuration(BattleVisualType visualType)
    {
        return visualType switch
        {
            BattleVisualType.AttackSpell => 0.48f,
            BattleVisualType.FireSpell => 0.58f,
            BattleVisualType.WaterSpell => 0.62f,
            BattleVisualType.WindSpell => 0.60f,
            BattleVisualType.CatchPulse => 0.70f,
            BattleVisualType.GuardShield => 0.80f,
            _ => 0f
        };
    }

    private string GetElementDisplayName(ElementType elementType)
    {
        return elementType switch
        {
            ElementType.Fire => "Fire",
            ElementType.Water => "Water",
            ElementType.Wind => "Wind",
            _ => "Unknown"
        };
    }

    private bool CurrentThemeIs(string themeName)
    {
        return string.Equals(_currentTheme?.Name, themeName, StringComparison.Ordinal);
    }

    private bool IsInvertedWaterTraversal()
    {
        return CurrentThemeIs("Water");
    }

    private string GetThemeChallengeSummary()
    {
        if (CurrentThemeIs("Fire"))
        {
            return "Basalt route + lava + turrets";
        }

        if (CurrentThemeIs("Wind"))
        {
            return "Crosswind + aerial route changes";
        }

        if (CurrentThemeIs("Water"))
        {
            return "Inverted cave + bubble lifts + water monsters";
        }

        if (CurrentThemeIs("Abyss"))
        {
            return "Cycle reset with pure boss pressure";
        }

        return "Base map exploration";
    }

    private void IncreaseDifficultyAndApplyTheme(EnemyDefinition enemy)
    {
        _difficultyTier++;
        ApplyThemeFromEnemy(enemy);
    }

    private Rectangle ToScreenRectangle(Rectangle worldRectangle)
    {
        return new Rectangle((int)(worldRectangle.X - _cameraX), worldRectangle.Y, worldRectangle.Width, worldRectangle.Height);
    }

    private Vector2 ToScreenPosition(Vector2 worldPosition)
    {
        return new Vector2(worldPosition.X - _cameraX, worldPosition.Y);
    }

    private void DrawWorldFilledRectangle(Rectangle worldRectangle, Color color)
    {
        _spriteBatch.Draw(_pixel, ToScreenRectangle(worldRectangle), color);
    }

    private void DrawWorldRectangleOutline(Rectangle worldRectangle, Color color, int thickness)
    {
        Rectangle screenRectangle = ToScreenRectangle(worldRectangle);
        DrawRectangleOutline(screenRectangle, color, thickness);
    }

    private void DrawWorldText(string text, Vector2 worldPosition, Color color)
    {
        _spriteBatch.DrawString(_font, text, ToScreenPosition(worldPosition), color);
    }

    private void DrawFilledRectangle(Rectangle rectangle, Color color)
    {
        _spriteBatch.Draw(_pixel, rectangle, color);
    }

    private void DrawRectangleOutline(Rectangle rectangle, Color color, int thickness)
    {
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, thickness), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, thickness, rectangle.Height), color);
        _spriteBatch.Draw(_pixel, new Rectangle(rectangle.Right - thickness, rectangle.Top, thickness, rectangle.Height), color);
    }

    private void DrawTextBlock(string text, Vector2 position, Color color)
    {
        _spriteBatch.DrawString(_font, text, position, color);
    }

    private void PlaySound(SoundEffect sound, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        sound.Play(MathHelper.Clamp(volume, 0f, 1f), MathHelper.Clamp(pitch, -1f, 1f), MathHelper.Clamp(pan, -1f, 1f));
    }

    private bool WasKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
    }

    private Color GetBackgroundColor()
    {
        return _gamePhase switch
        {
            GamePhase.Overworld => new Color(221, 239, 255),
            GamePhase.Battle => new Color(230, 224, 239),
            GamePhase.Victory => new Color(238, 248, 238),
            GamePhase.Defeat => new Color(249, 233, 233),
            _ => Color.CornflowerBlue
        };
    }

    private Rectangle PlayerBounds => new((int)_playerPosition.X, (int)_playerPosition.Y, PlayerWidth, PlayerHeight);

    private Rectangle PreviousPlayerBounds => new((int)_previousPlayerPosition.X, (int)_previousPlayerPosition.Y, PlayerWidth, PlayerHeight);
}

