using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IlkOyun;

public partial class Game1
{
    private void BuildOverworld()
    {
        _basePlatforms.Clear();
        _puzzleSwitches.Clear();
        _puzzleBarriers.Clear();
        _doors.Clear();

        BuildEnemyDoors();
        BuildStaticPlatforms();
        BuildPuzzleObjects();
    }

    private void BuildNeutralTheme()
    {
        _currentTheme = new WorldThemePalette(
            "Neutral",
            new Color(233, 246, 255),
            new Color(223, 237, 250),
            new Color(216, 228, 244),
            new Color(109, 208, 92),
            new Color(121, 213, 255),
            new Color(77, 112, 166));
    }

    private void ApplyThemeFromEnemy(EnemyDefinition enemy)
    {
        _currentTheme = enemy.RewardElement switch
        {
            ElementType.Fire => new WorldThemePalette(
                "Fire",
                new Color(255, 231, 224),
                new Color(252, 208, 194),
                new Color(240, 187, 176),
                new Color(231, 112, 78),
                new Color(255, 193, 139),
                new Color(196, 78, 52)),
            ElementType.Water => new WorldThemePalette(
                "Water",
                new Color(223, 241, 255),
                new Color(198, 225, 252),
                new Color(178, 206, 237),
                new Color(86, 170, 232),
                new Color(166, 225, 255),
                new Color(54, 112, 189)),
            ElementType.Wind => new WorldThemePalette(
                "Wind",
                new Color(236, 250, 231),
                new Color(216, 240, 205),
                new Color(196, 223, 182),
                new Color(120, 191, 107),
                new Color(207, 244, 184),
                new Color(87, 136, 74)),
            _ => new WorldThemePalette(
                "Abyss",
                new Color(236, 227, 255),
                new Color(214, 199, 245),
                new Color(188, 175, 228),
                new Color(146, 115, 221),
                new Color(238, 167, 232),
                new Color(118, 86, 182))
        };
    }

    private void BuildEnemyDoors()
    {
        EnemyDefinition fireMiniBoss = new(
            "Ember Sentinel",
            82,
            9,
            15,
            false,
            "The Ember Sentinel guards the fire seal. To gain its essence, you must capture it.",
            "The fire spirit answered your Ghost Tube.",
            new Color(240, 108, 61),
            new Color(255, 216, 171),
            ElementType.Fire,
            true);

        EnemyDefinition waterMiniBoss = new(
            "Tide Keeper",
            90,
            10,
            16,
            false,
            "The Tide Keeper rises from the gate. Capture it to earn the water essence.",
            "The water spirit has joined your summon list.",
            new Color(77, 169, 255),
            new Color(214, 244, 255),
            ElementType.Water,
            true);

        EnemyDefinition windMiniBoss = new(
            "Gale Wraith",
            86,
            9,
            17,
            false,
            "The Gale Wraith flickers through the wind gate. Only a successful catch grants its power.",
            "The wind spirit now empowers your catch mechanic.",
            new Color(183, 229, 169),
            new Color(246, 255, 240),
            ElementType.Wind,
            true);

        EnemyDefinition finalBoss = new(
            "Soul Monarch",
            118,
            10,
            17,
            true,
            "All three essences resonate. The Soul Monarch descends for the final battle.",
            "The final seal has broken and the world map loop is complete.",
            new Color(125, 82, 186),
            new Color(247, 161, 226),
            null,
            false);

        _doors.Add(new DoorDefinition(
            "Fire Gate",
            new Rectangle(1022, 428, 84, 112),
            fireMiniBoss,
            DoorType.MiniBoss,
            1,
            new Color(222, 90, 58),
            "Unlock with the first rune switch."));

        _doors.Add(new DoorDefinition(
            "Water Gate",
            new Rectangle(1960, 428, 84, 112),
            waterMiniBoss,
            DoorType.MiniBoss,
            2,
            new Color(65, 159, 244),
            "Unlock with the second rune switch."));

        _doors.Add(new DoorDefinition(
            "Wind Gate",
            new Rectangle(2576, 448, 84, 112),
            windMiniBoss,
            DoorType.MiniBoss,
            3,
            new Color(135, 193, 110),
            "Unlock with the third rune switch."));

        _doors.Add(new DoorDefinition(
            "Final Gate",
            new Rectangle(3826, 328, 94, 112),
            finalBoss,
            DoorType.FinalBoss,
            -1,
            new Color(135, 93, 196),
            "Opens only after all three mini-bosses are captured."));
    }

    private void BuildStaticPlatforms()
    {
        _basePlatforms.Clear();

        string themeName = _currentTheme?.Name ?? "Neutral";
        Rectangle[] layout = themeName switch
        {
            "Fire" => GetFirePlatformLayout(),
            "Wind" => GetWindPlatformLayout(),
            "Water" => GetWaterPlatformLayout(),
            "Abyss" => GetAbyssPlatformLayout(),
            _ => GetNeutralPlatformLayout()
        };

        _basePlatforms.AddRange(layout);
    }

    private Rectangle[] GetNeutralPlatformLayout()
    {
        return
        [
            new Rectangle(0, 620, 460, 100),
            new Rectangle(520, 580, 300, 140),
            new Rectangle(900, 540, 240, 180),
            new Rectangle(1240, 620, 260, 100),
            new Rectangle(1560, 580, 240, 140),
            new Rectangle(1860, 540, 280, 180),
            new Rectangle(2200, 620, 240, 100),
            new Rectangle(2500, 560, 240, 160),
            new Rectangle(2800, 620, 260, 100),
            new Rectangle(3140, 560, 220, 160),
            new Rectangle(3440, 500, 220, 220),
            new Rectangle(3740, 440, 260, 280),
            new Rectangle(380, 520, 100, 20),
            new Rectangle(760, 500, 90, 20),
            new Rectangle(1440, 520, 90, 20),
            new Rectangle(1780, 470, 90, 20),
            new Rectangle(2380, 520, 90, 20),
            new Rectangle(2720, 470, 90, 20),
            new Rectangle(3360, 430, 90, 20),
            new Rectangle(3580, 380, 90, 20)
        ];
    }

    private Rectangle[] GetFirePlatformLayout()
    {
        return
        [
            new Rectangle(0, 620, 150, 100),
            new Rectangle(230, 580, 90, 140),
            new Rectangle(380, 540, 90, 180),
            new Rectangle(540, 600, 80, 120),
            new Rectangle(640, 560, 90, 160),
            new Rectangle(780, 500, 70, 20),
            new Rectangle(900, 560, 70, 160),
            new Rectangle(1000, 540, 130, 180),
            new Rectangle(1200, 620, 90, 100),
            new Rectangle(1360, 570, 90, 150),
            new Rectangle(1500, 620, 80, 100),
            new Rectangle(1600, 560, 90, 160),
            new Rectangle(1740, 500, 70, 20),
            new Rectangle(1870, 560, 70, 160),
            new Rectangle(1940, 540, 130, 180),
            new Rectangle(2140, 620, 90, 100),
            new Rectangle(2260, 590, 100, 130),
            new Rectangle(2440, 620, 80, 100),
            new Rectangle(2520, 560, 130, 160),
            new Rectangle(2710, 500, 70, 20),
            new Rectangle(2860, 560, 90, 160),
            new Rectangle(3020, 620, 80, 100),
            new Rectangle(3180, 540, 90, 180),
            new Rectangle(3340, 500, 100, 220),
            new Rectangle(3500, 460, 90, 260),
            new Rectangle(3660, 420, 90, 300),
            new Rectangle(3820, 440, 180, 280)
        ];
    }

    private Rectangle[] GetWindPlatformLayout()
    {
        return
        [
            new Rectangle(0, 620, 140, 100),
            new Rectangle(210, 580, 80, 140),
            new Rectangle(340, 530, 90, 20),
            new Rectangle(500, 480, 90, 20),
            new Rectangle(620, 540, 70, 180),
            new Rectangle(760, 460, 90, 20),
            new Rectangle(900, 540, 120, 180),
            new Rectangle(1080, 490, 90, 20),
            new Rectangle(1220, 620, 100, 100),
            new Rectangle(1380, 560, 70, 160),
            new Rectangle(1500, 500, 100, 20),
            new Rectangle(1600, 560, 120, 160),
            new Rectangle(1760, 470, 90, 20),
            new Rectangle(1910, 540, 140, 180),
            new Rectangle(2140, 620, 80, 100),
            new Rectangle(2250, 580, 90, 140),
            new Rectangle(2390, 510, 90, 20),
            new Rectangle(2500, 560, 150, 160),
            new Rectangle(2710, 490, 90, 20),
            new Rectangle(2860, 440, 90, 20),
            new Rectangle(3020, 620, 80, 100),
            new Rectangle(3160, 540, 80, 180),
            new Rectangle(3290, 490, 90, 20),
            new Rectangle(3430, 440, 90, 20),
            new Rectangle(3560, 390, 100, 20),
            new Rectangle(3720, 440, 280, 280)
        ];
    }

    private Rectangle[] GetWaterPlatformLayout()
    {
        return
        [
            new Rectangle(0, 0, 4100, 70),
            new Rectangle(0, 688, 4100, 32),
            new Rectangle(260, 0, 70, 240),
            new Rectangle(560, 0, 180, 548),
            new Rectangle(760, 0, 70, 330),
            new Rectangle(940, 0, 180, 432),
            new Rectangle(1320, 0, 70, 250),
            new Rectangle(1540, 0, 220, 548),
            new Rectangle(1780, 0, 70, 360),
            new Rectangle(1880, 0, 220, 432),
            new Rectangle(2200, 0, 210, 590),
            new Rectangle(2480, 0, 220, 452),
            new Rectangle(2880, 0, 70, 320),
            new Rectangle(3300, 0, 70, 240),
            new Rectangle(3740, 0, 220, 332)
        ];
    }

    private Rectangle[] GetAbyssPlatformLayout()
    {
        return
        [
            new Rectangle(0, 620, 120, 100),
            new Rectangle(180, 570, 80, 150),
            new Rectangle(320, 620, 70, 100),
            new Rectangle(460, 540, 90, 180),
            new Rectangle(620, 500, 90, 20),
            new Rectangle(760, 560, 80, 160),
            new Rectangle(920, 540, 180, 180),
            new Rectangle(1170, 620, 70, 100),
            new Rectangle(1310, 560, 80, 160),
            new Rectangle(1470, 500, 90, 20),
            new Rectangle(1600, 560, 100, 160),
            new Rectangle(1780, 480, 80, 20),
            new Rectangle(1940, 540, 150, 180),
            new Rectangle(2140, 620, 70, 100),
            new Rectangle(2260, 580, 80, 140),
            new Rectangle(2420, 500, 90, 20),
            new Rectangle(2520, 560, 130, 160),
            new Rectangle(2690, 480, 90, 20),
            new Rectangle(2860, 420, 90, 20),
            new Rectangle(3040, 560, 100, 160),
            new Rectangle(3210, 480, 80, 20),
            new Rectangle(3360, 420, 90, 20),
            new Rectangle(3500, 360, 100, 20),
            new Rectangle(3640, 320, 120, 20),
            new Rectangle(3770, 440, 230, 280)
        ];
    }

    private Vector2 GetSpawnPositionForCurrentTheme()
    {
        return _currentTheme?.Name switch
        {
            "Fire" => new Vector2(10, 566),
            "Wind" => new Vector2(96, 566),
            "Water" => new Vector2(56, 108),
            "Abyss" => new Vector2(18, 566),
            _ => new Vector2(72, 566)
        };
    }

    private void BuildPuzzleObjects()
    {
        _puzzleSwitches.Add(new PuzzleSwitch(1, "Fire Rune", new Rectangle(646, 544, 36, 16), new Color(240, 108, 61)));
        _puzzleSwitches.Add(new PuzzleSwitch(2, "Water Rune", new Rectangle(1662, 544, 36, 16), new Color(77, 169, 255)));
        _puzzleSwitches.Add(new PuzzleSwitch(3, "Wind Rune", new Rectangle(2292, 584, 36, 16), new Color(183, 229, 169)));

        _puzzleBarriers.Add(new PuzzleBarrier(1, new Rectangle(980, 428, 20, 112), new Color(240, 108, 61)));
        _puzzleBarriers.Add(new PuzzleBarrier(2, new Rectangle(1918, 428, 20, 112), new Color(77, 169, 255)));
        _puzzleBarriers.Add(new PuzzleBarrier(3, new Rectangle(2534, 448, 20, 112), new Color(183, 229, 169)));
    }

    private void ResetFullRun()
    {
        _difficultyTier = 0;
        _cycleNumber = 1;
        _hasDoubleJumpUpgrade = false;
        _hasUsedDoubleJump = false;
        BuildNeutralTheme();
        ResetRunProgress();
        _statusMessage = "Explore the large map, activate rune puzzles, capture the three mini-bosses, then open the final gate.";
    }

    private void StartNextCycle()
    {
        _cycleNumber++;
        _hasDoubleJumpUpgrade = false;
        _hasUsedDoubleJump = false;
        ResetRunProgress();
        _statusMessage = $"Cycle {_cycleNumber} basladi. Harita artik daha tehlikeli ve {_currentTheme.Name} temasinda.";
    }

    private void BeginNextCycle()
    {
        _cycleNumber++;
        _hasDoubleJumpUpgrade = false;
        _hasUsedDoubleJump = false;
        ResetRunProgress();
        _statusMessage = $"Cycle {_cycleNumber} started. The map is now more dangerous and carries the {_currentTheme.Name} theme.";
    }

    private void ResetRunProgress()
    {
        _capturedElements.Clear();
        _campaignCompleted = false;

        foreach (PuzzleSwitch puzzleSwitch in _puzzleSwitches)
        {
            puzzleSwitch.Activated = false;
        }

        foreach (DoorDefinition door in _doors)
        {
            door.Completed = false;
        }

        ResetOverworldState();
    }

    private void ResetOverworldState()
    {
        _gamePhase = GamePhase.Overworld;
        _battleState = null;
        _activeDoor = null!;
        _returnToOverworldAfterResult = false;
        _spawnPosition = GetSpawnPositionForCurrentTheme();
        _playerPosition = _spawnPosition;
        _previousPlayerPosition = _spawnPosition;
        _playerVelocity = Vector2.Zero;
        _cameraX = 0f;
        _isOnGround = false;
        _isUnderwater = false;
        _jumpBufferTimer = 0f;
        _coyoteTimeTimer = 0f;
        _bubbleDiveGraceTimer = 0f;
        _waterDashTimer = 0f;
        _waterDashCooldownTimer = 0f;
        _waterDashDirection = 1f;
        _spawnProtectionTimer = 0.90f;
        _hasUsedDoubleJump = false;
        _upgradeDrops.Clear();

        BuildStaticPlatforms();
        RebuildMapEnemies();
        RebuildThemeThreats();
    }

    private void RebuildMapEnemies()
    {
        _mapEnemies.Clear();
        float speedBonus = _difficultyTier * 6f;
        Color enemyColor = CurrentThemeIs("Fire")
            ? new Color(164, 94, 68)
            : CurrentThemeIs("Water")
                ? new Color(70, 118, 158)
                : CurrentThemeIs("Wind")
                    ? new Color(102, 146, 88)
                    : CurrentThemeIs("Abyss")
                        ? new Color(94, 82, 132)
                        : new Color(126, 92, 58);

        if (CurrentThemeIs("Fire"))
        {
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(240, 540), 38, 40, 230, 280, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(646, 520), 38, 40, 640, 692, 88f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1010, 500), 38, 40, 1000, 1086, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1364, 530), 38, 40, 1360, 1412, 82f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1606, 520), 38, 40, 1600, 1652, 90f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1948, 500), 38, 40, 1940, 2022, 88f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2528, 520), 38, 40, 2520, 2602, 92f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(3346, 460), 38, 40, 3340, 3402, 86f + speedBonus, enemyColor));
            return;
        }

        if (CurrentThemeIs("Wind"))
        {
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(510, 440), 38, 40, 500, 552, 80f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(910, 500), 38, 40, 900, 982, 84f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1608, 520), 38, 40, 1600, 1682, 82f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1918, 500), 38, 40, 1910, 2012, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2252, 540), 38, 40, 2250, 2298, 88f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2508, 520), 38, 40, 2500, 2592, 90f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(3436, 400), 38, 40, 3430, 3476, 86f + speedBonus, enemyColor));
            return;
        }

        if (CurrentThemeIs("Water"))
        {
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(362, 176), 38, 40, 350, 450, 78f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(848, 270), 38, 40, 840, 900, 82f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1148, 382), 38, 40, 1140, 1230, 84f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1770, 506), 38, 40, 1760, 1840, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2122, 388), 38, 40, 2110, 2180, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2440, 548), 38, 40, 2428, 2472, 88f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2720, 412), 38, 40, 2710, 2790, 84f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(3384, 192), 38, 40, 3380, 3470, 82f + speedBonus, enemyColor));
            return;
        }

        if (CurrentThemeIs("Abyss"))
        {
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(188, 530), 38, 40, 180, 222, 84f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(468, 500), 38, 40, 460, 502, 88f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(924, 500), 38, 40, 920, 1042, 86f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1316, 520), 38, 40, 1310, 1352, 82f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(1948, 500), 38, 40, 1940, 2032, 90f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(2528, 520), 38, 40, 2520, 2602, 92f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(3048, 520), 38, 40, 3040, 3102, 90f + speedBonus, enemyColor));
            _mapEnemies.Add(new PatrollingEnemy(new Vector2(3508, 320), 38, 40, 3500, 3552, 92f + speedBonus, enemyColor));
            return;
        }

        _mapEnemies.Add(new PatrollingEnemy(new Vector2(150, 580), 38, 40, 90, 340, 78f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(610, 540), 38, 40, 560, 760, 84f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(930, 500), 38, 40, 920, 1080, 82f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(1610, 540), 38, 40, 1585, 1740, 80f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(1910, 500), 38, 40, 1890, 2070, 86f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(2530, 520), 38, 40, 2510, 2680, 88f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(3180, 520), 38, 40, 3160, 3310, 82f + speedBonus, enemyColor));
        _mapEnemies.Add(new PatrollingEnemy(new Vector2(3470, 460), 38, 40, 3460, 3610, 84f + speedBonus, enemyColor));
    }

    private void RebuildThemeThreats()
    {
        _themePlatforms.Clear();
        _lavaPools.Clear();
        _fireTurrets.Clear();
        _fireProjectiles.Clear();
        _flyingEnemies.Clear();
        _waterZones.Clear();
        _bubbleLiftZones.Clear();
        _dashFish.Clear();

        if (CurrentThemeIs("Fire"))
        {
            AddFireGapLavaPools();
            _fireTurrets.Add(new FireTurret(new Vector2(1412, 542), false, 1.55f, new Color(218, 84, 49)));
            _fireTurrets.Add(new FireTurret(new Vector2(1740, 438), false, 1.35f, new Color(218, 84, 49)));
            _fireTurrets.Add(new FireTurret(new Vector2(3500, 432), true, 1.10f, new Color(218, 84, 49)));
        }
        else if (CurrentThemeIs("Wind"))
        {
            _flyingEnemies.Add(new FlyingEnemy(new Vector2(716, 416), 660f, 880f, 92f + (_difficultyTier * 4f), 18f, 0.25f, new Color(191, 239, 186)));
            _flyingEnemies.Add(new FlyingEnemy(new Vector2(1678, 388), 1604f, 1828f, 95f + (_difficultyTier * 4f), 16f, 0.95f, new Color(191, 239, 186)));
            _flyingEnemies.Add(new FlyingEnemy(new Vector2(2724, 362), 2664f, 2868f, 102f + (_difficultyTier * 4f), 20f, 1.55f, new Color(191, 239, 186)));
            _flyingEnemies.Add(new FlyingEnemy(new Vector2(3388, 312), 3328f, 3606f, 110f + (_difficultyTier * 4f), 22f, 2.20f, new Color(191, 239, 186)));
        }
        else if (CurrentThemeIs("Water"))
        {
            _waterZones.Add(new WaterZone(new Rectangle(0, 70, WorldWidth, 618), new Color(82, 171, 255, 118)));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(344, 92, 58, 596), new Color(214, 244, 255, 110), 520f));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(850, 92, 58, 596), new Color(214, 244, 255, 110), 530f));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(1150, 92, 58, 596), new Color(214, 244, 255, 110), 540f));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(1762, 92, 58, 596), new Color(214, 244, 255, 110), 540f));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(2420, 92, 62, 596), new Color(214, 244, 255, 110), 560f));
            _bubbleLiftZones.Add(new BubbleLiftZone(new Rectangle(3368, 92, 62, 596), new Color(214, 244, 255, 110), 560f));

            _dashFish.Add(new DashFish(new Vector2(648, 612), 578f, 722f, 54f, 232f + (_difficultyTier * 10f), new Color(51, 118, 208)));
            _dashFish.Add(new DashFish(new Vector2(1624, 612), 1562f, 1738f, 58f, 240f + (_difficultyTier * 10f), new Color(51, 118, 208)));
            _dashFish.Add(new DashFish(new Vector2(2284, 632), 2218f, 2394f, 60f, 246f + (_difficultyTier * 10f), new Color(51, 118, 208)));
            _dashFish.Add(new DashFish(new Vector2(3476, 520), 3402f, 3590f, 60f, 250f + (_difficultyTier * 10f), new Color(51, 118, 208)));
        }
    }

    private void AddFireGapLavaPools()
    {
        List<Rectangle> sortedPlatforms = new(_basePlatforms);
        sortedPlatforms.Sort((left, right) => left.Left.CompareTo(right.Left));

        for (int index = 0; index < sortedPlatforms.Count - 1; index++)
        {
            Rectangle current = sortedPlatforms[index];
            Rectangle next = sortedPlatforms[index + 1];
            int gapWidth = next.Left - current.Right;

            if (gapWidth < 26)
            {
                continue;
            }

            int lavaX = current.Right;
            int lavaWidth = gapWidth;
            int lavaTop = Math.Max(current.Top, next.Top) + 8;
            int lavaHeight = WindowHeight - lavaTop;

            _lavaPools.Add(new HazardZone(new Rectangle(lavaX, lavaTop, lavaWidth, lavaHeight), new Color(248, 118, 59), "Lava"));
        }
    }
}
