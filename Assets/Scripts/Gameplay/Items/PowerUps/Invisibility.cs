using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    public class Invisibility : PowerUpBase
    {
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
            if (cachedPowerups != null)
            {
                cachedPowerups.SetInvisible(false);
            }
        }
    }
}
