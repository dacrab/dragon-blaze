using UnityEngine;
using UnityEngine.InputSystem;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Core.State;
using Gameplay.Combat;
using Gameplay.Characters.Player;

namespace Core.Debug
{
    /// <summary>
    /// Cheat commands (F1=Heal, F2=+100 coins, F3=KillAll, F4=GodMode, F5=Save, F9=SpeedToggle).
    /// Stripped from release builds via asmdef defineConstraints.
    /// </summary>
    public sealed class DebugConsole : MonoBehaviour
    {
        Player player;
        Health playerHealth;
        bool godMode;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            if (FindFirstObjectByType<DebugConsole>() != null) return;
            var console = new GameObject(nameof(DebugConsole)).AddComponent<DebugConsole>();
            DontDestroyOnLoad(console.gameObject);
        }

        void Update()
        {
            if (playerHealth == null) ResolvePlayer();
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.f1Key.wasPressedThisFrame) playerHealth?.Heal(playerHealth.MaxHealth);
            if (keyboard.f2Key.wasPressedThisFrame) ServiceLocator.Get<IGameManager>()?.AddCoins(100);
            if (keyboard.f3Key.wasPressedThisFrame) KillAllEnemies();
            if (keyboard.f4Key.wasPressedThisFrame) ToggleGodMode();
            if (keyboard.f5Key.wasPressedThisFrame) ServiceLocator.Get<IGameManager>()?.SaveGame();
            if (keyboard.f9Key.wasPressedThisFrame) ToggleSpeed();
        }

        void ToggleSpeed()
        {
            // Leave paused/game-over time alone: forcing 3x behind the overlay resumes the world.
            if (ServiceLocator.Get<IGameStateManager>()?.IsPlaying != true) return;
            Time.timeScale = Time.timeScale > 1 ? 1 : 3;
        }

        void ResolvePlayer()
        {
            var playerTransform = GameConstants.FindPlayer();
            if (playerTransform == null) return;
            player = playerTransform.GetComponent<Player>();
            playerHealth = playerTransform.GetComponent<Health>();
            // Re-apply god mode to the freshly resolved player (new level) so the toggle stays meaningful.
            if (godMode) playerHealth?.SetInvulnerable(true);
        }

        static void KillAllEnemies()
        {
            foreach (var enemy in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (enemy.CompareTag(GameConstants.Tags.Enemy)) enemy.TakeDamage(9999);
        }

        void ToggleGodMode()
        {
            godMode = !godMode;
            // Real damage immunity, not invisibility: god mode previously made the player translucent but still hurt.
            playerHealth?.SetInvulnerable(godMode);
        }
    }
}
