using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IlkOyun;

public partial class Game1
{
    private void StartBattle(DoorDefinition door)
    {
        int hpBonus = _difficultyTier * (door.Enemy.IsBoss ? 3 : 4);

        _activeDoor = door;
        _battleState = new BattleState
        {
            Enemy = door.Enemy,
            PlayerHp = 100,
            PlayerMaxHp = 100,
            EnemyHp = door.Enemy.MaxHp + hpBonus,
            EnemyMaxHp = door.Enemy.MaxHp + hpBonus,
            CapturedElementCount = _capturedElements.Count,
            FireAvailable = _capturedElements.Contains(ElementType.Fire),
            WaterAvailable = _capturedElements.Contains(ElementType.Water),
            WindAvailable = _capturedElements.Contains(ElementType.Wind),
            Message = $"{door.Enemy.IntroMessage} Difficulty tier: {_difficultyTier}."
        };

        _gamePhase = GamePhase.Battle;
        PlaySound(_battleEnterSound, 0.50f, door.Enemy.IsBoss ? -0.10f : 0.05f);
    }

    private void UpdateBattle(GameTime gameTime)
    {
        if (_battleState is null)
        {
            return;
        }

        UpdateBattleVisualTimers((float)gameTime.ElapsedGameTime.TotalSeconds);

        if (WasKeyPressed(Keys.D1) || WasKeyPressed(Keys.NumPad1))
        {
            ExecuteAttack();
        }
        else if (WasKeyPressed(Keys.D2) || WasKeyPressed(Keys.NumPad2))
        {
            ExecuteGuard();
        }
        else if (WasKeyPressed(Keys.D3) || WasKeyPressed(Keys.NumPad3))
        {
            ExecuteCatch();
        }
        else if (WasKeyPressed(Keys.D4) || WasKeyPressed(Keys.NumPad4))
        {
            ExecuteFireSummon();
        }
        else if (WasKeyPressed(Keys.D5) || WasKeyPressed(Keys.NumPad5))
        {
            ExecuteWaterSummon();
        }
        else if (WasKeyPressed(Keys.D6) || WasKeyPressed(Keys.NumPad6))
        {
            ExecuteWindSummon();
        }
    }

    private void ExecuteAttack()
    {
        PlaySound(_spellSound, 0.34f, 0.02f);
        TriggerBattleVisual(BattleVisualType.AttackSpell, 0.48f);

        int damage = 12 + _random.Next(6) + (_battleState.CapturedElementCount * 2);
        _battleState.EnemyHp = Math.Max(0, _battleState.EnemyHp - damage);
        _battleState.Message = $"Basic attack dealt {damage} damage.";
        TriggerEnemyImpact(0.22f);

        if (TryForceMiniBossCaptureWindow())
        {
            return;
        }

        if (_battleState.EnemyHp <= 0)
        {
            ResolveBattleVictory(false, "defeated");
            return;
        }

        ResolveEnemyTurn();
    }

    private void ExecuteGuard()
    {
        _battleState.IsGuarding = true;
        _battleState.Message = "Guard activated. Incoming damage will be reduced this turn.";
        PlaySound(_spellSound, 0.26f, -0.30f);
        TriggerBattleVisual(BattleVisualType.GuardShield, 0.80f);
        _battleState.GuardVisualTimer = 0.80f;
        ResolveEnemyTurn();
    }

    private void ExecuteCatch()
    {
        if (_activeDoor.DoorType == DoorType.FinalBoss)
        {
            _battleState.Message = "Soul Monarch yakalanamaz. Onu dusurmek icin ruh formunu yok etmelisin.";
            return;
        }

        TriggerBattleVisual(BattleVisualType.CatchPulse, 0.70f);
        float catchChance = GetCatchChance();
        bool captured = _random.NextSingle() <= catchChance;

        if (captured)
        {
            ResolveBattleVictory(true, "captured");
            return;
        }

        _battleState.Message = $"Catch failed at {(int)(catchChance * 100)}% chance. The enemy retaliates.";
        PlaySound(_catchFailSound, 0.40f, -0.08f);
        ResolveEnemyTurn();
    }

    private void ExecuteFireSummon()
    {
        if (!_battleState.FireAvailable)
        {
            _battleState.Message = "Fire summon is not available in this battle.";
            return;
        }

        _battleState.FireAvailable = false;
        PlaySound(_spellSound, 0.42f, 0.20f);
        TriggerBattleVisual(BattleVisualType.FireSpell, 0.58f);
        int damage = 25 + _random.Next(8);
        _battleState.EnemyHp = Math.Max(0, _battleState.EnemyHp - damage);
        _battleState.Message = $"Fire summon dealt {damage} burst damage.";
        TriggerEnemyImpact(0.28f);

        if (TryForceMiniBossCaptureWindow())
        {
            return;
        }

        if (_battleState.EnemyHp <= 0)
        {
            ResolveBattleVictory(false, "overpowered");
            return;
        }

        ResolveEnemyTurn();
    }

    private void ExecuteWaterSummon()
    {
        if (!_battleState.WaterAvailable)
        {
            _battleState.Message = "Water summon is not available in this battle.";
            return;
        }

        _battleState.WaterAvailable = false;
        PlaySound(_spellSound, 0.38f, -0.04f);
        TriggerBattleVisual(BattleVisualType.WaterSpell, 0.62f);
        int damage = 12 + _random.Next(6);
        int healAmount = 15 + _random.Next(7);
        int recoveredHp = Math.Min(healAmount, _battleState.PlayerMaxHp - _battleState.PlayerHp);

        _battleState.EnemyHp = Math.Max(0, _battleState.EnemyHp - damage);
        _battleState.PlayerHp = Math.Min(_battleState.PlayerMaxHp, _battleState.PlayerHp + healAmount);
        _battleState.Message = $"Water summon dealt {damage} damage and restored {recoveredHp} HP.";
        TriggerEnemyImpact(0.24f);

        if (TryForceMiniBossCaptureWindow())
        {
            return;
        }

        if (_battleState.EnemyHp <= 0)
        {
            ResolveBattleVictory(false, "washed away");
            return;
        }

        ResolveEnemyTurn();
    }

    private void ExecuteWindSummon()
    {
        if (!_battleState.WindAvailable)
        {
            _battleState.Message = "Wind summon is not available in this battle.";
            return;
        }

        _battleState.WindAvailable = false;
        PlaySound(_spellSound, 0.36f, 0.14f);
        TriggerBattleVisual(BattleVisualType.WindSpell, 0.60f);
        int damage = 9 + _random.Next(5);
        _battleState.EnemyHp = Math.Max(0, _battleState.EnemyHp - damage);
        _battleState.CatchBonus = MathF.Min(_battleState.CatchBonus + WindCatchBonus, 0.40f);
        _battleState.Message = $"Wind summon dealt {damage} damage and increased catch chance by {(int)(WindCatchBonus * 100)}%.";
        TriggerEnemyImpact(0.20f);

        if (TryForceMiniBossCaptureWindow())
        {
            return;
        }

        if (_battleState.EnemyHp <= 0)
        {
            ResolveBattleVictory(false, "scattered");
            return;
        }

        ResolveEnemyTurn();
    }

    private bool TryForceMiniBossCaptureWindow()
    {
        if (_activeDoor.DoorType != DoorType.MiniBoss || !_battleState.Enemy.RequiresCatchForCompletion || _battleState.EnemyHp > 0)
        {
            return false;
        }

        int forcedCatchHp = Math.Max(1, (int)MathF.Ceiling(_battleState.EnemyMaxHp * GuaranteedCatchThreshold));
        _battleState.EnemyHp = forcedCatchHp;
        _battleState.CatchBonus = MathF.Max(_battleState.CatchBonus, 1f);
        _battleState.Message = $"{_battleState.Message} {_battleState.Enemy.Name} could not be destroyed; its spirit form is pinned down. Catch is now guaranteed.";
        return true;
    }

    private void ResolveEnemyTurn()
    {
        EnemyDefinition enemy = _battleState.Enemy;
        PlaySound(_enemyCastSound, 0.34f, enemy.IsBoss ? -0.20f : -0.06f);
        int damage = enemy.MinDamage + _random.Next(enemy.MaxDamage - enemy.MinDamage + 1) + _difficultyTier;

        if (_battleState.IsGuarding)
        {
            damage = Math.Max(1, damage / 2);
        }

        _battleState.PlayerHp = Math.Max(0, _battleState.PlayerHp - damage);
        _battleState.IsGuarding = false;
        _battleState.Message = $"{_battleState.Message} {enemy.Name} dealt {damage} damage.";
        TriggerPlayerImpact(0.24f);

        if (_battleState.PlayerHp <= 0)
        {
            _statusMessage = "The mage was defeated.";
            _returnToOverworldAfterResult = true;
            _gamePhase = GamePhase.Defeat;
        }
    }

    private void ResolveBattleVictory(bool captured, string resolution)
    {
        EnemyDefinition enemy = _battleState.Enemy;
        _returnToOverworldAfterResult = true;
        PlaySound(captured ? _catchSuccessSound : _victorySound, captured ? 0.46f : 0.50f, captured ? 0.06f : 0f);

        if (_activeDoor.DoorType == DoorType.MiniBoss)
        {
            if (captured && enemy.RewardElement.HasValue)
            {
                _capturedElements.Add(enemy.RewardElement.Value);
                _activeDoor.Completed = true;
                IncreaseDifficultyAndApplyTheme(enemy);
                _statusMessage = $"{enemy.Name} was captured. {GetElementDisplayName(enemy.RewardElement.Value)} essence is now available and stored for the final gate. Theme changed to {_currentTheme.Name} and difficulty increased to {_difficultyTier}.";
                _gamePhase = GamePhase.Victory;
                return;
            }

            if (enemy.RequiresCatchForCompletion)
            {
                _statusMessage = $"{enemy.Name} was {resolution}, but it was not captured. The door resets because the element essence was not collected.";
                _gamePhase = GamePhase.Victory;
                return;
            }
        }

        _activeDoor.Completed = true;
        if (_activeDoor.DoorType == DoorType.FinalBoss)
        {
            IncreaseDifficultyAndApplyTheme(enemy);
            _campaignCompleted = true;
            _returnToOverworldAfterResult = false;
            _statusMessage = $"Soul Monarch was {resolution}. Final boss dustu ve dongu tamamlandi. Yeni tur {_currentTheme.Name} temasinda difficulty tier {_difficultyTier} ile baslayacak.";
            _gamePhase = GamePhase.Victory;
            return;
        }

        _statusMessage = $"{enemy.Name} was {resolution}.";
        _gamePhase = GamePhase.Victory;
    }

    private void UpdateResultScreen()
    {
        if (!WasKeyPressed(Keys.Enter))
        {
            return;
        }

        if (_campaignCompleted)
        {
            BeginNextCycle();
            return;
        }

        if (_returnToOverworldAfterResult)
        {
            ResetOverworldState();
            return;
        }

        ResetFullRun();
    }
}
