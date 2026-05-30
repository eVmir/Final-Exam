using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IlkOyun;

public partial class Game1
{
    private void DrawOverworld()
    {
        DrawOverworldBackground();

        foreach (Rectangle platform in _basePlatforms)
        {
            DrawWorldFilledRectangle(platform, new Color(122, 79, 42));
            DrawWorldFilledRectangle(new Rectangle(platform.X, platform.Y, platform.Width, 12), _currentTheme.PlatformTop);
            DrawWorldRectangleOutline(platform, new Color(87, 53, 24), 2);
        }

        DrawThemeEnvironment();

        foreach (PuzzleBarrier barrier in _puzzleBarriers)
        {
            if (IsSwitchActivated(barrier.SwitchId))
            {
                continue;
            }

            DrawPuzzleBarrier(barrier);
        }

        foreach (PuzzleSwitch puzzleSwitch in _puzzleSwitches)
        {
            DrawPuzzleSwitch(puzzleSwitch);
        }

        foreach (DoorDefinition door in _doors)
        {
            DrawDoor(door);
        }

        foreach (PatrollingEnemy enemy in _mapEnemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            DrawPatrollingEnemy(enemy);
        }

        DrawThemeActors();

        foreach (UpgradeDrop drop in _upgradeDrops)
        {
            DrawUpgradeDrop(drop);
        }

        DrawPlayer();
        DrawOverworldHud();
    }

    private void DrawOverworldBackground()
    {
        DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, 240), _currentTheme.SkyTop);
        DrawFilledRectangle(new Rectangle(0, 240, WindowWidth, 170), _currentTheme.SkyMiddle);
        DrawFilledRectangle(new Rectangle(0, 410, WindowWidth, 310), _currentTheme.Horizon);

        for (int index = 0; index < 8; index++)
        {
            int worldX = 180 + (index * 500);
            int screenX = (int)(worldX - (_cameraX * 0.35f));
            Rectangle hill = new(screenX, 360 + (index % 2 == 0 ? 0 : 26), 240, 130);
            DrawFilledRectangle(hill, new Color(
                (byte)Math.Max(0, _currentTheme.Horizon.R - 22),
                (byte)Math.Max(0, _currentTheme.Horizon.G - 18),
                (byte)Math.Max(0, _currentTheme.Horizon.B - 8)));
        }

        Color farSilhouette = new(
            (byte)Math.Max(0, _currentTheme.Horizon.R - 34),
            (byte)Math.Max(0, _currentTheme.Horizon.G - 30),
            (byte)Math.Max(0, _currentTheme.Horizon.B - 22),
            (byte)255);
        Color midSilhouette = new(
            (byte)Math.Max(0, _currentTheme.Horizon.R - 18),
            (byte)Math.Max(0, _currentTheme.Horizon.G - 14),
            (byte)Math.Max(0, _currentTheme.Horizon.B - 10),
            (byte)255);
        DrawBackdropRuins(0.16f, farSilhouette);
        DrawBackdropRuins(0.24f, midSilhouette);
        DrawThemeBackdropDetails();

        if (CurrentThemeIs("Water"))
        {
            DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, WindowHeight), new Color(72, 146, 214, 48));

            for (int index = 0; index < 9; index++)
            {
                int bubbleX = 80 + (index * 130) - (int)(_cameraX * 0.18f % 50f);
                int bubbleY = 120 + ((index * 67) % 420);
                DrawFilledRectangle(new Rectangle(bubbleX, bubbleY, 8, 8), new Color(220, 246, 255, 180));
                DrawFilledRectangle(new Rectangle(bubbleX + 14, bubbleY + 26, 5, 5), new Color(220, 246, 255, 130));
            }
        }
    }

    private void DrawBackdropRuins(float parallax, Color color)
    {
        for (int index = 0; index < 7; index++)
        {
            int worldX = 120 + (index * 620) + ((index % 2) * 54);
            int groundY = 452 + ((index % 3) * 16);
            int width = 56 + ((index % 3) * 12);
            int height = 84 + ((index % 4) * 22);

            DrawBackdropTower(worldX, groundY, width, height, parallax, color);

            if (index % 2 == 0)
            {
                DrawBackdropArch(worldX + 94, groundY + 10, 86, 54, parallax, color);
            }
        }
    }

    private void DrawThemeBackdropDetails()
    {
        Color accent = new(
            (byte)Math.Max(0, _currentTheme.UiAccent.R - 14),
            (byte)Math.Max(0, _currentTheme.UiAccent.G - 14),
            (byte)Math.Max(0, _currentTheme.UiAccent.B - 14),
            (byte)255);

        if (CurrentThemeIs("Fire"))
        {
            DrawBackdropSpire(310, 452, 72, 124, 0.22f, accent);
            DrawBackdropSpire(1420, 472, 84, 152, 0.20f, accent);
            DrawBackdropSpire(2790, 440, 94, 176, 0.18f, accent);
            DrawBackdropSpire(3560, 430, 78, 148, 0.24f, accent);
            return;
        }

        if (CurrentThemeIs("Wind"))
        {
            DrawBackdropFloatingIsland(460, 244, 168, 28, 0.18f, accent);
            DrawBackdropFloatingIsland(1710, 214, 148, 24, 0.22f, accent);
            DrawBackdropFloatingIsland(3020, 188, 174, 26, 0.20f, accent);
            DrawBackdropTower(980, 424, 28, 104, 0.28f, accent);
            DrawBackdropTower(3320, 404, 32, 126, 0.26f, accent);
            return;
        }

        if (CurrentThemeIs("Water"))
        {
            DrawBackdropCoralPillar(280, 500, 58, 114, 0.18f, accent);
            DrawBackdropCoralPillar(1260, 522, 64, 132, 0.16f, accent);
            DrawBackdropCoralPillar(2320, 490, 70, 142, 0.18f, accent);
            DrawBackdropCoralPillar(3440, 468, 66, 150, 0.20f, accent);
            return;
        }

        if (CurrentThemeIs("Abyss"))
        {
            DrawBackdropSpire(420, 450, 62, 142, 0.20f, accent);
            DrawBackdropSpire(1650, 430, 78, 176, 0.18f, accent);
            DrawBackdropSpire(2890, 452, 70, 156, 0.22f, accent);
            DrawBackdropSpire(3680, 408, 88, 198, 0.20f, accent);
            return;
        }

        DrawBackdropTower(260, 476, 42, 96, 0.24f, accent);
        DrawBackdropTower(1880, 458, 48, 118, 0.20f, accent);
        DrawBackdropTower(3240, 438, 44, 136, 0.22f, accent);
    }

    private void DrawBackdropTower(int worldX, int groundY, int width, int height, float parallax, Color color)
    {
        DrawParallaxRectangle(new Rectangle(worldX, groundY - height, width, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX - 6, groundY - height, width + 12, 10), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + (width / 3), groundY - height + 18, 8, height - 28), parallax, BlendColor(color, Color.Black, 0.18f));
    }

    private void DrawBackdropArch(int worldX, int groundY, int width, int height, float parallax, Color color)
    {
        DrawParallaxRectangle(new Rectangle(worldX, groundY - height, 18, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + width - 18, groundY - height, 18, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX, groundY - height, width, 16), parallax, color);
    }

    private void DrawBackdropFloatingIsland(int worldX, int worldY, int width, int height, float parallax, Color color)
    {
        DrawParallaxRectangle(new Rectangle(worldX, worldY, width, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + 18, worldY + height, width - 36, 12), parallax, BlendColor(color, Color.Black, 0.12f));
        DrawParallaxRectangle(new Rectangle(worldX + (width / 2) - 10, worldY + height + 12, 20, 14), parallax, BlendColor(color, Color.Black, 0.18f));
    }

    private void DrawBackdropCoralPillar(int worldX, int groundY, int width, int height, float parallax, Color color)
    {
        DrawParallaxRectangle(new Rectangle(worldX, groundY - height, width, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX - 14, groundY - height + 26, 14, 20), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + width, groundY - height + 18, 16, 24), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + 10, groundY - height - 8, width - 20, 12), parallax, BlendColor(color, Color.White, 0.08f));
    }

    private void DrawBackdropSpire(int worldX, int groundY, int width, int height, float parallax, Color color)
    {
        DrawParallaxRectangle(new Rectangle(worldX, groundY - height, width, height), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + 10, groundY - height - 18, width - 20, 18), parallax, color);
        DrawParallaxRectangle(new Rectangle(worldX + (width / 2) - 6, groundY - height - 36, 12, 18), parallax, color);
    }

    private void DrawParallaxRectangle(Rectangle worldRectangle, float parallax, Color color)
    {
        Rectangle screenRectangle = new(
            (int)(worldRectangle.X - (_cameraX * parallax)),
            worldRectangle.Y,
            worldRectangle.Width,
            worldRectangle.Height);
        DrawFilledRectangle(screenRectangle, color);
    }

    private void DrawThemeEnvironment()
    {
        foreach (WaterZone waterZone in _waterZones)
        {
            DrawWorldFilledRectangle(waterZone.Bounds, waterZone.Color);
            DrawWorldFilledRectangle(new Rectangle(waterZone.Bounds.X, waterZone.Bounds.Y, waterZone.Bounds.Width, 8), new Color(205, 239, 255, 180));
            DrawWorldRectangleOutline(waterZone.Bounds, new Color(44, 112, 182), 2);

            for (int stripe = 0; stripe < 3; stripe++)
            {
                int y = waterZone.Bounds.Y + 16 + (stripe * 18);
                DrawWorldFilledRectangle(new Rectangle(waterZone.Bounds.X + 10, y, waterZone.Bounds.Width - 20, 2), new Color(214, 244, 255, 125));
            }
        }

        foreach (BubbleLiftZone bubbleLiftZone in _bubbleLiftZones)
        {
            DrawBubbleLiftZone(bubbleLiftZone);
        }

        foreach (ThemePlatformPiece platformPiece in _themePlatforms)
        {
            DrawWorldFilledRectangle(platformPiece.Bounds, platformPiece.BodyColor);
            DrawWorldFilledRectangle(new Rectangle(platformPiece.Bounds.X, platformPiece.Bounds.Y, platformPiece.Bounds.Width, Math.Min(10, platformPiece.Bounds.Height)), platformPiece.TopColor);
            DrawWorldRectangleOutline(platformPiece.Bounds, new Color(36, 36, 44), 2);
        }

        foreach (HazardZone lavaPool in _lavaPools)
        {
            DrawLavaPool(lavaPool);
        }
    }

    private void DrawLavaPool(HazardZone lavaPool)
    {
        Rectangle pool = lavaPool.Bounds;
        Rectangle surface = new(pool.X, pool.Y, pool.Width, Math.Min(14, pool.Height));
        Rectangle core = new(pool.X, pool.Y + surface.Height, pool.Width, Math.Max(1, pool.Height - surface.Height));
        Rectangle deepCore = new(pool.X, pool.Y + (pool.Height / 2), pool.Width, Math.Max(1, pool.Height / 2));

        DrawWorldFilledRectangle(pool, new Color(176, 28, 24));
        DrawWorldFilledRectangle(core, new Color(194, 36, 28));
        DrawWorldFilledRectangle(deepCore, new Color(118, 12, 12));
        DrawWorldFilledRectangle(surface, new Color(255, 82, 46));

        int streakWidth = Math.Max(8, pool.Width / 5);
        DrawWorldFilledRectangle(new Rectangle(pool.X + 6, pool.Y + 3, streakWidth, 4), new Color(255, 122, 72));
        DrawWorldFilledRectangle(new Rectangle(pool.Right - streakWidth - 6, pool.Y + 5, streakWidth, 3), new Color(236, 64, 42));
    }

    private void DrawBubbleLiftZone(BubbleLiftZone bubbleLiftZone)
    {
        DrawWorldFilledRectangle(bubbleLiftZone.Bounds, bubbleLiftZone.Color);

        for (int bubble = 0; bubble < 8; bubble++)
        {
            int offsetY = (int)((_animationTimer * 90f) + (bubble * 68f)) % Math.Max(1, bubbleLiftZone.Bounds.Height);
            int bubbleY = bubbleLiftZone.Bounds.Bottom - 12 - offsetY;
            int bubbleX = bubbleLiftZone.Bounds.X + 10 + ((bubble * 9) % Math.Max(12, bubbleLiftZone.Bounds.Width - 20));
            int size = bubble % 3 == 0 ? 10 : 6;
            DrawWorldFilledRectangle(new Rectangle(bubbleX, bubbleY, size, size), new Color(236, 250, 255, 180));
        }

        DrawWorldRectangleOutline(bubbleLiftZone.Bounds, new Color(198, 234, 255, 120), 2);
    }

    private void DrawPuzzleBarrier(PuzzleBarrier barrier)
    {
        int pulse = (int)((MathF.Sin((_animationTimer * 5f) + barrier.Bounds.X) + 1f) * 10f);
        Rectangle glow = new(barrier.Bounds.X - 4, barrier.Bounds.Y - 4, barrier.Bounds.Width + 8, barrier.Bounds.Height + 8);
        Color glowColor = new(barrier.Color.R, barrier.Color.G, barrier.Color.B, (byte)(120 + pulse));

        DrawWorldFilledRectangle(glow, glowColor);
        DrawWorldFilledRectangle(barrier.Bounds, barrier.Color);
        DrawWorldRectangleOutline(barrier.Bounds, Color.White, 2);
    }

    private void DrawPuzzleSwitch(PuzzleSwitch puzzleSwitch)
    {
        Color fillColor = puzzleSwitch.Activated ? puzzleSwitch.Color : new Color(185, 185, 185);
        DrawWorldFilledRectangle(puzzleSwitch.Bounds, fillColor);
        DrawWorldRectangleOutline(puzzleSwitch.Bounds, Color.Black, 2);
        DrawWorldText(puzzleSwitch.Activated ? "ON" : "OFF", new Vector2(puzzleSwitch.Bounds.X - 8, puzzleSwitch.Bounds.Y - 22), Color.Black);
    }

    private void DrawDoor(DoorDefinition door)
    {
        bool unlocked = IsDoorUnlocked(door);
        Rectangle outer = door.Bounds;
        Rectangle inner = new(door.Bounds.X + 8, door.Bounds.Y + 8, door.Bounds.Width - 16, door.Bounds.Height - 16);
        Color innerColor = unlocked ? _currentTheme.Glow : new Color(94, 94, 120);

        DrawWorldFilledRectangle(outer, door.FrameColor);
        DrawWorldFilledRectangle(inner, innerColor);
        DrawWorldRectangleOutline(outer, Color.White, 3);

        if (door.Completed && door.DoorType == DoorType.MiniBoss)
        {
            DrawWorldText("Captured", new Vector2(door.Bounds.X - 8, door.Bounds.Y - 28), new Color(42, 121, 63));
        }
        else if (!unlocked)
        {
            DrawWorldText("Locked", new Vector2(door.Bounds.X + 8, door.Bounds.Y - 28), new Color(151, 66, 66));
        }
        else
        {
            DrawWorldText("Ready", new Vector2(door.Bounds.X + 10, door.Bounds.Y - 28), new Color(35, 98, 156));
        }

        DrawWorldText(door.Name, new Vector2(door.Bounds.X - 18, door.Bounds.Y - 48), Color.Black);
    }

    private void DrawPatrollingEnemy(PatrollingEnemy enemy)
    {
        DrawActorShadow(enemy.Bounds, 34, 9, -2, new Color(0, 0, 0, 74));
        Rectangle spriteBounds = new(enemy.Bounds.X - 28, enemy.Bounds.Y - 34, enemy.Bounds.Width + 58, enemy.Bounds.Height + 40);
        Texture2D sprite = GetGroundEnemyTexture();
        DrawWorldCharacterSprite(sprite, spriteBounds, 1.10f, true, enemy.MoveRight, Color.White);
    }

    private void DrawThemeActors()
    {
        foreach (FireTurret turret in _fireTurrets)
        {
            DrawFireTurret(turret);
        }

        foreach (FireProjectile projectile in _fireProjectiles)
        {
            DrawWorldFilledRectangle(projectile.Bounds, projectile.Color);
            DrawWorldFilledRectangle(new Rectangle(projectile.Bounds.X + 4, projectile.Bounds.Y - 4, 8, 4), new Color(255, 232, 161));
        }

        foreach (FlyingEnemy enemy in _flyingEnemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            DrawFlyingEnemy(enemy);
        }

        foreach (DashFish fish in _dashFish)
        {
            if (!fish.IsAlive)
            {
                continue;
            }

            DrawWaterMonster(fish);
        }
    }

    private void DrawFireTurret(FireTurret turret)
    {
        Rectangle body = turret.Bounds;
        Rectangle muzzle = turret.ShootLeft
            ? new Rectangle(body.X - 10, body.Y + 8, 10, 8)
            : new Rectangle(body.Right, body.Y + 8, 10, 8);

        DrawWorldFilledRectangle(body, turret.Color);
        DrawWorldFilledRectangle(new Rectangle(body.X + 6, body.Y + 4, body.Width - 12, 10), new Color(255, 201, 125));
        DrawWorldFilledRectangle(muzzle, new Color(121, 51, 27));
        DrawWorldRectangleOutline(body, Color.Black, 2);
    }

    private void DrawFlyingEnemy(FlyingEnemy enemy)
    {
        DrawActorShadow(enemy.Bounds, 32, 8, 20, new Color(0, 0, 0, 54));
        Rectangle spriteBounds = new(enemy.Bounds.X - 40, enemy.Bounds.Y - 52, enemy.Bounds.Width + 82, enemy.Bounds.Height + 74);
        Color tint = BlendColor(Color.White, enemy.BodyColor, 0.12f);
        DrawWorldCharacterSprite(_windWraithTexture, spriteBounds, 1.06f, true, enemy.MoveRight, tint);
    }

    private void DrawWaterMonster(DashFish fish)
    {
        DrawActorShadow(fish.Bounds, 38, 10, 2, new Color(0, 0, 0, 48));
        Rectangle spriteBounds = new(fish.Bounds.X - 38, fish.Bounds.Y - 58, fish.Bounds.Width + 84, fish.Bounds.Height + 86);
        Color tint = BlendColor(Color.White, fish.BodyColor, 0.14f);
        DrawWorldCharacterSprite(_waterGolemTexture, spriteBounds, 1.08f, true, fish.MoveRight, tint);
    }

    private void DrawUpgradeDrop(UpgradeDrop drop)
    {
        DrawWorldFilledRectangle(drop.Bounds, drop.Color);
        DrawWorldRectangleOutline(drop.Bounds, Color.Black, 2);
        DrawWorldText(drop.Label, new Vector2(drop.Position.X + 2, drop.Position.Y + 4), Color.Black);
    }

    private void DrawPlayer()
    {
        Rectangle body = PlayerBounds;
        int bobOffset = Math.Abs(_playerVelocity.X) > 6f && _isOnGround
            ? (int)(MathF.Sin(_animationTimer * 11f) * 2f)
            : (int)(MathF.Sin(_animationTimer * 2.6f) * 1.5f);
        DrawActorShadow(body, 38, 10, -2, new Color(0, 0, 0, 80));
        Rectangle spriteBounds = new(body.X - 40, body.Y - 78 + bobOffset, body.Width + 96, body.Height + 92);
        DrawWorldCharacterSprite(_overworldMageTexture, spriteBounds, 1.12f, true, _playerFacingRight, Color.White);
    }

    private void DrawOverworldHud()
    {
        DrawElementSlot(ElementType.Fire, new Rectangle(1088, 24, 44, 44), _capturedElements.Contains(ElementType.Fire), new Color(240, 108, 61));
        DrawElementSlot(ElementType.Water, new Rectangle(1148, 24, 44, 44), _capturedElements.Contains(ElementType.Water), new Color(77, 169, 255));
        DrawElementSlot(ElementType.Wind, new Rectangle(1208, 24, 44, 44), _capturedElements.Contains(ElementType.Wind), new Color(183, 229, 169));
    }

    private void DrawElementSlot(ElementType type, Rectangle slot, bool unlocked, Color color)
    {
        DrawFilledRectangle(slot, unlocked ? color : new Color(212, 212, 212, 180));
        DrawRectangleOutline(slot, unlocked ? Color.White : new Color(118, 118, 118), 2);

        Rectangle icon = new(slot.X + 10, slot.Y + 10, slot.Width - 20, slot.Height - 20);
        DrawFilledRectangle(icon, unlocked ? new Color(255, 255, 255, 120) : new Color(120, 120, 120, 80));
    }

    private void DrawBattle()
    {
        EnemyDefinition enemy = _battleState.Enemy;

        _spriteBatch.Draw(_battleBackgroundTexture, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
        DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, WindowHeight), new Color(_currentTheme.UiAccent.R, _currentTheme.UiAccent.G, _currentTheme.UiAccent.B, (byte)40));
        DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, 132), new Color(10, 12, 28, 120));
        DrawFilledRectangle(new Rectangle(0, WindowHeight - 210, WindowWidth, 210), new Color(10, 12, 28, 170));

        Rectangle playerCard = new(42, 34, 336, 124);
        Rectangle enemyCard = new(902, 34, 336, 124);

        DrawBattlePanel(playerCard, new Color(12, 18, 34, 168), new Color(120, 190, 255, 110));
        DrawBattlePanel(enemyCard, new Color(18, 14, 30, 168), new Color(enemy.PrimaryColor.R, enemy.PrimaryColor.G, enemy.PrimaryColor.B, (byte)126));

        DrawTextBlock("Rabbit Mage", new Vector2(70, 54), Color.White);
        DrawTextBlock($"HP {_battleState.PlayerHp} / {_battleState.PlayerMaxHp}", new Vector2(70, 84), new Color(214, 232, 255));
        DrawHealthBar(new Rectangle(70, 132, 262, 18), _battleState.PlayerHp, _battleState.PlayerMaxHp, new Color(88, 169, 255));

        DrawTextBlock(enemy.Name, new Vector2(930, 54), Color.White);
        DrawTextBlock($"HP {_battleState.EnemyHp} / {_battleState.EnemyMaxHp}", new Vector2(930, 84), new Color(230, 221, 248));
        DrawHealthBar(new Rectangle(930, 132, 262, 18), _battleState.EnemyHp, _battleState.EnemyMaxHp, enemy.PrimaryColor);

        Rectangle mageArea = new(84, 168, 304, 270);
        Rectangle spiritArea = new(816, 142, 362, 314);

        DrawFilledRectangle(new Rectangle(mageArea.X + 30, mageArea.Bottom - 12, mageArea.Width - 60, 16), new Color(0, 0, 0, 68));
        DrawFilledRectangle(new Rectangle(spiritArea.X + 44, spiritArea.Bottom - 16, spiritArea.Width - 88, 18), new Color(0, 0, 0, 82));

        DrawBattleMage(mageArea);
        DrawSpiritFigure(spiritArea, enemy);
        DrawBattleEffects(mageArea, spiritArea, enemy);

        string battleControls = "1 Attack   2 Guard   3 Catch   4 Fire   5 Water   6 Wind";
        Vector2 controlsPosition = new(338, 680);
        DrawTextBlock(battleControls, controlsPosition + new Vector2(1f, 1f), new Color(0, 0, 0, 120));
        DrawTextBlock(battleControls, controlsPosition, new Color(232, 238, 255, 220));
    }

    private void DrawTitleScreen()
    {
        DrawOverworldBackground();
        DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, WindowHeight), new Color(8, 10, 22, 144));

        Rectangle titlePanel = new(154, 110, 972, 484);
        Rectangle titleAccent = new(titlePanel.X + 18, titlePanel.Y + 18, titlePanel.Width - 36, titlePanel.Height - 36);
        DrawFilledRectangle(titlePanel, new Color(9, 14, 30, 196));
        DrawRectangleOutline(titlePanel, new Color(186, 204, 255, 150), 2);
        DrawFilledRectangle(titleAccent, new Color(18, 27, 52, 160));

        float pulse = (MathF.Sin(_animationTimer * 2.4f) + 1f) * 0.5f;
        Color glow = new(
            (byte)MathHelper.Lerp(165, 220, pulse),
            (byte)MathHelper.Lerp(192, 236, pulse),
            (byte)MathHelper.Lerp(255, 255, pulse),
            (byte)255);

        DrawTextBlock("SOUL HUNTER", new Vector2(362, 170), glow);
        DrawTextBlock("Platform adventure with elemental spirit battles", new Vector2(318, 224), new Color(211, 220, 244));

        Rectangle leftShowcase = new(202, 288, 270, 220);
        Rectangle rightShowcase = new(810, 270, 280, 246);
        DrawFilledRectangle(new Rectangle(leftShowcase.X + 34, leftShowcase.Bottom - 10, leftShowcase.Width - 68, 14), new Color(0, 0, 0, 68));
        DrawFilledRectangle(new Rectangle(rightShowcase.X + 40, rightShowcase.Bottom - 12, rightShowcase.Width - 80, 16), new Color(0, 0, 0, 72));
        Rectangle titleMageBounds = new(212 + (int)(MathF.Sin(_animationTimer * 1.8f) * 4f), 236, 260, 278);
        Rectangle titleMageDestination = GetFittedTextureRectangle(_overworldMageTexture, titleMageBounds, 1.02f, true);
        _spriteBatch.Draw(_overworldMageTexture, titleMageDestination, Color.White);
        DrawSpiritFigure(rightShowcase, new EnemyDefinition(
            "Title Spirit",
            1,
            1,
            1,
            true,
            string.Empty,
            string.Empty,
            new Color(146, 115, 221),
            new Color(238, 167, 232),
            null,
            false));

        DrawFilledRectangle(new Rectangle(404, 358, 468, 64), new Color(255, 255, 255, 10));
        DrawRectangleOutline(new Rectangle(404, 358, 468, 64), new Color(188, 200, 235, 58), 2);
        Vector2 startTextPosition = new(510, 378);
        DrawTextBlock("Enter  Start Game", startTextPosition + new Vector2(1f, 1f), new Color(0, 0, 0, 120));
        DrawTextBlock("Enter  Start Game", startTextPosition, new Color(214, 220, 234));
        DrawTextBlock("Esc  Exit", new Vector2(576, 456), new Color(176, 184, 206));
    }

    private void DrawResultScreen()
    {
        bool isVictory = _gamePhase == GamePhase.Victory;
        DrawFilledRectangle(new Rectangle(0, 0, WindowWidth, WindowHeight), new Color(10, 12, 24, 210));

        Rectangle panel = new(188, 136, 904, 432);
        Rectangle innerPanel = new(panel.X + 18, panel.Y + 18, panel.Width - 36, panel.Height - 36);
        DrawFilledRectangle(panel, isVictory ? new Color(26, 44, 40, 232) : new Color(54, 28, 32, 232));
        DrawRectangleOutline(panel, isVictory ? new Color(157, 236, 194, 140) : new Color(255, 182, 194, 140), 2);
        DrawFilledRectangle(innerPanel, isVictory ? new Color(37, 64, 56, 210) : new Color(72, 36, 40, 210));

        DrawTextBlock(isVictory ? "Cycle Resolved" : "Soul Lost", new Vector2(460, 196), Color.WhiteSmoke);
        DrawWrappedText(_statusMessage, new Rectangle(260, 276, 760, 120), new Color(224, 229, 240));
        DrawTextBlock("Enter  Continue", new Vector2(512, 464), new Color(208, 216, 236));
    }

    private void DrawSummonRow(string title, bool isAvailable, string detail, Vector2 position)
    {
        Color stateColor = isAvailable ? new Color(42, 121, 63) : new Color(151, 66, 66);
        DrawTextBlock($"{title}: {(isAvailable ? "Ready" : "Locked/Used")}", position, stateColor);
        DrawTextBlock(detail, new Vector2(position.X, position.Y + 22), Color.Black);
    }

    private void DrawBattleMage(Rectangle area)
    {
        int bobOffset = (int)(MathF.Sin((_animationTimer * 2.15f) + 0.8f) * 5f);
        Rectangle idleArea = new(area.X, area.Y + bobOffset, area.Width, area.Height);
        Rectangle destination = GetFittedTextureRectangle(_battleMageTexture, idleArea, 1.04f, true);

        _spriteBatch.Draw(_battleMageTexture, destination, Color.White);
    }

    private void DrawSpiritFigure(Rectangle area, EnemyDefinition enemy)
    {
        int swayOffset = (int)(MathF.Sin((_animationTimer * 2.8f) + area.X) * 6f);
        Rectangle floatArea = new(area.X, area.Y + swayOffset, area.Width, area.Height);
        Rectangle destination = GetFittedTextureRectangle(_battleGhostTexture, floatArea, enemy.IsBoss ? 1.10f : 0.94f, true);
        Color ghostTint = GetGhostSpriteTint(enemy);

        _spriteBatch.Draw(_battleGhostTexture, destination, ghostTint);

        Rectangle accentBody = new(destination.X + 24, destination.Y + 30, destination.Width - 48, destination.Height - 42);
        Rectangle accentHead = new(destination.X + 48, destination.Y + 10, destination.Width - 96, 44);
        DrawElementGhostAccents(floatArea, enemy, accentBody, accentHead, swayOffset);
    }

    private void DrawElementGhostAccents(Rectangle area, EnemyDefinition enemy, Rectangle body, Rectangle head, int swayOffset)
    {
        Color accentColor = GetGhostCoreColor(enemy);

        switch (enemy.RewardElement)
        {
            case ElementType.Fire:
                DrawFilledRectangle(new Rectangle(head.X + 8, head.Y - 12, 14, 12), accentColor);
                DrawFilledRectangle(new Rectangle(head.X + 28, head.Y - 20, 18, 20), new Color(255, 208, 132));
                DrawFilledRectangle(new Rectangle(head.Right - 24, head.Y - 12, 14, 12), accentColor);
                DrawFilledRectangle(new Rectangle(body.X + 8, body.Y + 10, 12, 16), new Color(255, 191, 133, 200));
                DrawFilledRectangle(new Rectangle(body.Right - 20, body.Y + 20, 10, 14), new Color(255, 191, 133, 200));
                break;
            case ElementType.Water:
                DrawFilledRectangle(new Rectangle(head.X + 22, head.Y - 16, 16, 16), accentColor);
                DrawFilledRectangle(new Rectangle(body.X + 14, body.Bottom - 2, 14, 18), new Color(204, 242, 255, 200));
                DrawFilledRectangle(new Rectangle(body.Right - 28, body.Bottom + 4, 14, 18), new Color(204, 242, 255, 200));
                DrawFilledRectangle(new Rectangle(body.X + 40, body.Y + 4, body.Width - 80, 8), new Color(214, 244, 255, 160));
                break;
            case ElementType.Wind:
                DrawFilledRectangle(new Rectangle(head.X - 8, head.Y + 8, 18, 6), new Color(231, 255, 234, 220));
                DrawFilledRectangle(new Rectangle(head.Right - 10, head.Y + 18, 18, 6), new Color(231, 255, 234, 220));
                DrawFilledRectangle(new Rectangle(body.X - 16, body.Y + 28, 20, 6), accentColor);
                DrawFilledRectangle(new Rectangle(body.Right - 4, body.Y + 10, 20, 6), accentColor);
                DrawFilledRectangle(new Rectangle(body.X + 22, body.Bottom + 2, body.Width - 44, 6), new Color(231, 255, 234, 180));
                break;
            default:
                DrawFilledRectangle(new Rectangle(head.X - 10, head.Y - 10, 12, 12), accentColor);
                DrawFilledRectangle(new Rectangle(head.Right - 2, head.Y - 10, 12, 12), accentColor);
                DrawFilledRectangle(new Rectangle(head.X + 22, head.Y - 20, 18, 12), new Color(255, 228, 255));
                DrawFilledRectangle(new Rectangle(body.X + 6, body.Y + 12, 10, 42), new Color(255, 218, 248, 160));
                DrawFilledRectangle(new Rectangle(body.Right - 16, body.Y + 18, 10, 42), new Color(255, 218, 248, 160));
                break;
        }

        if (enemy.IsBoss)
        {
            Rectangle crown = new(area.X + 88, area.Y - 2 + swayOffset, area.Width - 176, 12);
            DrawFilledRectangle(crown, new Color(235, 188, 255));
            DrawFilledRectangle(new Rectangle(crown.X + 10, crown.Y - 8, 10, 8), accentColor);
            DrawFilledRectangle(new Rectangle(crown.X + 34, crown.Y - 14, 12, 14), new Color(255, 239, 255));
            DrawFilledRectangle(new Rectangle(crown.Right - 20, crown.Y - 8, 10, 8), accentColor);
            DrawRectangleOutline(crown, Color.White, 2);
        }
    }

    private Color GetGhostCoreColor(EnemyDefinition enemy)
    {
        return enemy.RewardElement switch
        {
            ElementType.Fire => new Color(255, 209, 120),
            ElementType.Water => new Color(210, 245, 255),
            ElementType.Wind => new Color(229, 255, 223),
            _ => new Color(255, 220, 248)
        };
    }

    private Color GetGhostSpriteTint(EnemyDefinition enemy)
    {
        return enemy.RewardElement switch
        {
            ElementType.Fire => new Color(255, 228, 228),
            ElementType.Water => new Color(226, 244, 255),
            ElementType.Wind => new Color(236, 255, 232),
            _ => enemy.IsBoss
                ? new Color(248, 232, 255)
                : Color.White
        };
    }

    private Rectangle GetFittedTextureRectangle(Texture2D texture, Rectangle bounds, float scaleMultiplier, bool alignBottom)
    {
        float widthScale = bounds.Width / (float)texture.Width;
        float heightScale = bounds.Height / (float)texture.Height;
        float scale = MathF.Min(widthScale, heightScale) * scaleMultiplier;
        int width = Math.Max(1, (int)(texture.Width * scale));
        int height = Math.Max(1, (int)(texture.Height * scale));
        int x = bounds.X + ((bounds.Width - width) / 2);
        int y = alignBottom
            ? bounds.Bottom - height
            : bounds.Y + ((bounds.Height - height) / 2);

        return new Rectangle(x, y, width, height);
    }

    private void DrawWorldCharacterSprite(Texture2D texture, Rectangle worldBounds, float scaleMultiplier, bool alignBottom, bool facingRight, Color tint)
    {
        Rectangle destination = GetFittedTextureRectangle(texture, ToScreenRectangle(worldBounds), scaleMultiplier, alignBottom);
        SpriteEffects effects = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        _spriteBatch.Draw(texture, destination, null, tint, 0f, Vector2.Zero, effects, 0f);
    }

    private Texture2D GetGroundEnemyTexture()
    {
        if (CurrentThemeIs("Fire"))
        {
            return _fireGroundEnemyTexture;
        }

        if (CurrentThemeIs("Water"))
        {
            return _waterGroundEnemyTexture;
        }

        if (CurrentThemeIs("Wind"))
        {
            return _windGroundEnemyTexture;
        }

        if (CurrentThemeIs("Abyss"))
        {
            return _abyssGroundEnemyTexture;
        }

        return _roamingSpiritTexture;
    }

    private void DrawActorShadow(Rectangle anchorBounds, int width, int height, int verticalOffset, Color color)
    {
        Rectangle shadowBounds = new(
            anchorBounds.Center.X - (width / 2),
            anchorBounds.Bottom + verticalOffset,
            width,
            height);
        DrawWorldFilledRectangle(shadowBounds, color);
    }

    private Color BlendColor(Color from, Color to, float amount)
    {
        return new Color(
            (byte)MathHelper.Lerp(from.R, to.R, amount),
            (byte)MathHelper.Lerp(from.G, to.G, amount),
            (byte)MathHelper.Lerp(from.B, to.B, amount),
            (byte)MathHelper.Lerp(from.A, to.A, amount));
    }

    private void DrawBattlePanel(Rectangle area, Color fill, Color border)
    {
        DrawFilledRectangle(area, fill);
        DrawRectangleOutline(area, border, 2);
    }

    private void DrawCommandChip(Rectangle area, string title, string detail, bool enabled, Color accent)
    {
        Color fill = enabled ? new Color(24, 31, 54, 210) : new Color(34, 34, 42, 190);
        Color border = enabled ? new Color(accent.R, accent.G, accent.B, (byte)180) : new Color(108, 108, 124, 120);
        Color titleColor = enabled ? Color.WhiteSmoke : new Color(170, 170, 182);
        Color detailColor = enabled ? new Color(196, 210, 240) : new Color(126, 126, 142);

        DrawBattlePanel(area, fill, border);
        DrawTextBlock(title, new Vector2(area.X + 12, area.Y + 7), titleColor);
        DrawTextBlock(detail, new Vector2(area.X + 12, area.Y + 24), detailColor);
    }

    private void DrawWrappedText(string text, Rectangle bounds, Color color)
    {
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string line = string.Empty;
        float y = bounds.Y;

        foreach (string word in words)
        {
            string testLine = string.IsNullOrEmpty(line) ? word : $"{line} {word}";
            if (_font.MeasureString(testLine).X <= bounds.Width)
            {
                line = testLine;
                continue;
            }

            DrawTextBlock(line, new Vector2(bounds.X, y), color);
            y += _font.LineSpacing;
            line = word;

            if (y + _font.LineSpacing > bounds.Bottom)
            {
                return;
            }
        }

        if (!string.IsNullOrEmpty(line) && y + _font.LineSpacing <= bounds.Bottom + 2)
        {
            DrawTextBlock(line, new Vector2(bounds.X, y), color);
        }
    }

    private void DrawBattleEffects(Rectangle mageArea, Rectangle spiritArea, EnemyDefinition enemy)
    {
        Vector2 mageCenter = new(mageArea.X + (mageArea.Width / 2f), mageArea.Y + (mageArea.Height / 2.25f));
        Vector2 spiritCenter = new(spiritArea.X + (spiritArea.Width / 2f), spiritArea.Y + (spiritArea.Height / 2.4f));

        if (_battleState.ActiveVisual != BattleVisualType.None && _battleState.VisualTimer > 0f)
        {
            DrawActiveBattleVisual(mageArea, spiritArea, enemy);
        }

        if (_battleState.PlayerImpactTimer > 0f)
        {
            float progress = 1f - (_battleState.PlayerImpactTimer / 0.24f);
            Vector2 effectPoint = Vector2.Lerp(spiritCenter, mageCenter, MathHelper.Clamp(progress, 0f, 1f));
            DrawEnemyAttackVisual(effectPoint, enemy);
        }
    }

    private void DrawActiveBattleVisual(Rectangle mageArea, Rectangle spiritArea, EnemyDefinition enemy)
    {
        float duration = GetBattleVisualDuration(_battleState.ActiveVisual);
        float progress = duration <= 0f ? 1f : 1f - (_battleState.VisualTimer / duration);
        Vector2 mageCenter = new(mageArea.X + (mageArea.Width / 2f), mageArea.Y + (mageArea.Height / 2.25f));
        Vector2 spiritCenter = new(spiritArea.X + (spiritArea.Width / 2f), spiritArea.Y + (spiritArea.Height / 2.4f));
        Vector2 effectPoint = Vector2.Lerp(mageCenter, spiritCenter, MathHelper.Clamp(progress, 0f, 1f));

        switch (_battleState.ActiveVisual)
        {
            case BattleVisualType.AttackSpell:
                DrawProjectileTrail(mageCenter, effectPoint, new Color(168, 120, 255), 10, 4);
                DrawEffectOrb(effectPoint, 16, new Color(168, 120, 255));
                break;
            case BattleVisualType.FireSpell:
                DrawProjectileTrail(mageCenter, effectPoint, new Color(245, 122, 52), 12, 5);
                DrawEffectOrb(effectPoint, 22, new Color(245, 122, 52));
                break;
            case BattleVisualType.WaterSpell:
                DrawProjectileTrail(mageCenter, effectPoint, new Color(82, 171, 255), 11, 4);
                DrawEffectOrb(effectPoint, 18, new Color(82, 171, 255));
                break;
            case BattleVisualType.WindSpell:
                DrawProjectileTrail(mageCenter, effectPoint, new Color(172, 229, 172), 9, 4);
                DrawWindSlash(effectPoint, new Color(172, 229, 172));
                break;
            case BattleVisualType.CatchPulse:
                int pulseRadius = 34 + (int)(progress * 44f);
                byte pulseAlpha = (byte)(210 - (progress * 150f));
                DrawPulseRectangle(spiritCenter, pulseRadius, 54, new Color((byte)248, (byte)248, (byte)248, pulseAlpha), 3);
                DrawPulseRectangle(spiritCenter, pulseRadius + 18, 70, new Color(enemy.PrimaryColor.R, enemy.PrimaryColor.G, enemy.PrimaryColor.B, (byte)(pulseAlpha / 2)), 2);
                break;
            case BattleVisualType.GuardShield:
                break;
        }
    }

    private void DrawHealthBar(Rectangle area, int currentHp, int maxHp, Color fillColor)
    {
        DrawFilledRectangle(area, new Color(45, 45, 45));

        int filledWidth = maxHp == 0 ? 0 : (int)(area.Width * (currentHp / (float)maxHp));
        Rectangle fillArea = new(area.X + 3, area.Y + 3, Math.Max(0, filledWidth - 6), area.Height - 6);
        DrawFilledRectangle(fillArea, fillColor);
        DrawRectangleOutline(area, Color.White, 2);
        DrawTextBlock($"{currentHp} / {maxHp}", new Vector2(area.X + 10, area.Y + 2), Color.White);
    }

    private void DrawEffectOrb(Vector2 center, int size, Color color)
    {
        DrawFilledRectangle(new Rectangle((int)center.X - (size / 2), (int)center.Y - (size / 2), size, size), color);
    }

    private void DrawWindSlash(Vector2 center, Color color)
    {
        DrawFilledRectangle(new Rectangle((int)center.X - 22, (int)center.Y - 4, 44, 8), color);
        DrawFilledRectangle(new Rectangle((int)center.X - 10, (int)center.Y - 18, 26, 6), color);
    }

    private void DrawPulseRectangle(Vector2 center, int width, int height, Color color, int thickness)
    {
        Rectangle rectangle = new((int)center.X - (width / 2), (int)center.Y - (height / 2), width, height);
        DrawRectangleOutline(rectangle, color, thickness);
    }

    private void DrawProjectileTrail(Vector2 from, Vector2 to, Color color, int orbSize, int tailThickness)
    {
        Vector2 direction = to - from;
        float length = direction.Length();
        if (length <= 1f)
        {
            DrawEffectOrb(to, orbSize, color);
            return;
        }

        direction /= length;
        for (int index = 0; index < 4; index++)
        {
            float trailProgress = MathF.Max(0f, 1f - (index * 0.16f));
            Vector2 point = to - (direction * (index * 26f));
            int size = Math.Max(4, orbSize - (index * 3));
            byte alpha = (byte)Math.Clamp(220 - (index * 46), 0, 255);
            DrawFilledRectangle(
                new Rectangle((int)point.X - (size / 2), (int)point.Y - (tailThickness / 2), size, tailThickness),
                new Color(color.R, color.G, color.B, alpha));
            DrawFilledRectangle(
                new Rectangle((int)point.X - (size / 2), (int)point.Y - (size / 2), size, size),
                new Color(color.R, color.G, color.B, (byte)(alpha * trailProgress)));
        }
    }

    private void DrawEnemyAttackVisual(Vector2 effectPoint, EnemyDefinition enemy)
    {
        Color projectileColor = enemy.RewardElement switch
        {
            ElementType.Fire => new Color(255, 154, 98),
            ElementType.Water => new Color(132, 210, 255),
            ElementType.Wind => new Color(198, 244, 188),
            _ => new Color(228, 196, 255)
        };

        DrawEffectOrb(effectPoint, enemy.IsBoss ? 20 : 16, projectileColor);
        DrawFilledRectangle(new Rectangle((int)effectPoint.X - 18, (int)effectPoint.Y - 3, 24, 6), new Color(projectileColor.R, projectileColor.G, projectileColor.B, (byte)180));
    }
}
