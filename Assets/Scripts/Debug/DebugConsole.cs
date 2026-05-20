using UnityEngine;
using Core.Managers;
using Gameplay.Combat;

namespace DragonBlaze.Debug
{
    /// <summary>
    /// Development-only cheat commands. Stripped from release builds via asmdef defineConstraints.
    /// F1=Heal, F2=+100coins, F3=KillAll, F4=Invincible, F5=Save, F9=SpeedToggle
    /// </summary>
    public sealed class DebugConsole : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) HealPlayer();
            if (Input.GetKeyDown(KeyCode.F2)) GameManager.Instance?.AddCoins(100);
            if (Input.GetKeyDown(KeyCode.F3)) KillAllEnemies();
            if (Input.GetKeyDown(KeyCode.F4)) ToggleInvincibility();
            if (Input.GetKeyDown(KeyCode.F5)) GameManager.Instance?.SaveGame();
            if (Input.GetKeyDown(KeyCode.F9)) Time.timeScale = Time.timeScale > 1 ? 1 : 3;
        }

        static void HealPlayer()
        {
            var health = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>();
            health?.Heal(health.MaxHealth);
        }

        static void KillAllEnemies()
        {
            foreach (var enemy in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (enemy.CompareTag("Enemy")) enemy.TakeDamage(9999);
        }

        static void ToggleInvincibility()
        {
            var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Gameplay.Characters.Player.Player>();
            player?.SetInvisibility(!player.IsInvisible);
        }
    }
}
