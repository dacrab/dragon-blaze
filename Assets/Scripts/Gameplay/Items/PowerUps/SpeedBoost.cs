using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    public class SpeedBoost : PowerUpBase
    {
        [SerializeField] private Sprite speedBoostImage;
        [SerializeField] private float speedMultiplier = 2f;

        private float originalSpeed;
        private PlayerLocomotion locomotion;

        protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
        {
            locomotion = playerPowerups.GetComponent<PlayerLocomotion>();
            if (locomotion != null)
            {
                originalSpeed = locomotion.GetSpeed();
                locomotion.SetSpeed(originalSpeed * speedMultiplier);
            }
            ActivateIndicator("Speed Boost", speedBoostImage);
        }

        protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
        {
            if (locomotion != null)
            {
                locomotion.SetSpeed(originalSpeed);
            }
        }
    }
}
