using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// Speed boost power-up that temporarily increases player movement speed.
    /// </summary>
    public class SpeedBoost : PowerUpBase
    {
        [Header("Speed Boost Settings")]
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
            locomotion?.SetSpeed(originalSpeed);
            locomotion = null;
        }
    }
}
