using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// Invisibility power-up that makes the player invisible to enemies.
    /// </summary>
    public class Invisibility : PowerUpBase
    {
        [Header("Invisibility Settings")]
        [SerializeField] private Sprite invisibilityImage;

        private PlayerPowerups cachedPowerups;

        protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
        {
            cachedPowerups = playerPowerups;
            cachedPowerups.SetInvisible(true);
            ActivateIndicator("Invisibility", invisibilityImage);
        }

        protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
        {
            cachedPowerups?.SetInvisible(false);
            cachedPowerups = null;
        }
    }
}
