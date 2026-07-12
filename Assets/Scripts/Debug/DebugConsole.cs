using UnityEngine;
using Core.Constants;
using Core.Managers;
using Gameplay.Combat;
using Gameplay.Characters.Player;

namespace DragonBlaze.Debug
{
    /// <summary>
    /// Development-only cheat commands. Stripped from release builds via asmdef defineConstraints.
    /// F1=Heal, F2=+100coins, F3=KillAll, F4=Invincible, F5=Save, F9=SpeedToggle
    /// </summary>
    public sealed class DebugConsole : MonoBehaviour
    {
        Player player;
        Health playerHealth;

        void Awake()
        {
            var go = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player);
            if (go != null)
            {
                player = go.GetComponent<Player>();
                playerHealth = go.GetComponent<Health>();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) playerHealth?.Heal(playerHealth.MaxHealth);
            if (Input.GetKeyDown(KeyCode.F2)) GameManager.Instance?.AddCoins(100);
            if (Input.GetKeyDown(KeyCode.F3)) KillAllEnemies();
            if (Input.GetKeyDown(KeyCode.F4)) ToggleInvincibility();
            if (Input.GetKeyDown(KeyCode.F5)) GameManager.Instance?.SaveGame();
            if (Input.GetKeyDown(KeyCode.F9)) Time.timeScale = Time.timeScale > 1 ? 1 : 3;
        }

        static void KillAllEnemies()
        {
            foreach (var enemy in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (enemy.CompareTag(GameConstants.Tags.Enemy)) enemy.TakeDamage(9999);
        }

        void ToggleInvincibility() => player?.SetInvisibility(!player.IsInvisible);
    }
}
