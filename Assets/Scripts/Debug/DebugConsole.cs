using UnityEngine;
using UnityEngine.InputSystem;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Gameplay.Combat;
using Gameplay.Characters.Player;

namespace Core.Debug
{
    /// <summary>
    /// Cheat commands (F1=Heal, F2=+100 coins, F3=KillAll, F4=Invincible, F5=Save, F9=SpeedToggle).
    /// Stripped from release builds via asmdef defineConstraints.
    /// </summary>
    public sealed class DebugConsole : MonoBehaviour
    {
        Player player;
        Health playerHealth;

        void Awake()
        {
            var playerTransform = GameConstants.FindPlayer();
            if (playerTransform != null)
            {
                player = playerTransform.GetComponent<Player>();
                playerHealth = playerTransform.GetComponent<Health>();
            }
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard.f1Key.wasPressedThisFrame) playerHealth?.Heal(playerHealth.MaxHealth);
            if (keyboard.f2Key.wasPressedThisFrame) ServiceLocator.Get<IGameManager>()?.AddCoins(100);
            if (keyboard.f3Key.wasPressedThisFrame) KillAllEnemies();
            if (keyboard.f4Key.wasPressedThisFrame) ToggleInvincibility();
            if (keyboard.f5Key.wasPressedThisFrame) ServiceLocator.Get<IGameManager>()?.SaveGame();
            if (keyboard.f9Key.wasPressedThisFrame) Time.timeScale = Time.timeScale > 1 ? 1 : 3;
        }

        static void KillAllEnemies()
        {
            foreach (var enemy in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (enemy.CompareTag(GameConstants.Tags.Enemy)) enemy.TakeDamage(9999);
        }

        void ToggleInvincibility() => player?.SetInvisibility(!player.IsInvisible);
    }
}
