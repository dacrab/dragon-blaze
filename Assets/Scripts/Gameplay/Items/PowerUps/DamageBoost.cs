using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// Damage boost power-up that temporarily increases player attack damage.
    /// </summary>
    public class DamageBoost : PowerUpBase
    {
        [Header("Damage Boost Settings")]
        [SerializeField] private Sprite damageBoostImage;
        [SerializeField] private float damageMultiplier = 2f;

        private float originalDamage;
        private PlayerAttack playerAttack;

        protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
        {
            playerAttack = playerPowerups.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                originalDamage = playerAttack.GetDamage();
                playerAttack.SetDamage(originalDamage * damageMultiplier);
            }
            ActivateIndicator("Damage Boost", damageBoostImage);
        }

        protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
        {
            playerAttack?.SetDamage(originalDamage);
            playerAttack = null;
        }
    }
}
