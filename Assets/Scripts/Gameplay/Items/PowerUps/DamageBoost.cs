using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    public class DamageBoost : PowerUpBase
    {
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
