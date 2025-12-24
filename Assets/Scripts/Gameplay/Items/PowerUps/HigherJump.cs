using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    /// <summary>
    /// Higher jump power-up that temporarily increases player jump power.
    /// </summary>
    public class HigherJump : PowerUpBase
    {
        [Header("Higher Jump Settings")]
        [SerializeField] private float jumpMultiplier = 1.5f;
        [SerializeField] private Sprite higherJumpImage;

        private float originalJumpPower;
        private PlayerLocomotion locomotion;

        protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
        {
            locomotion = playerPowerups.GetComponent<PlayerLocomotion>();
            if (locomotion != null)
            {
                originalJumpPower = locomotion.GetJumpPower();
                locomotion.SetJumpPower(originalJumpPower * jumpMultiplier);
            }
            ActivateIndicator("Higher Jump", higherJumpImage);
        }

        protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
        {
            locomotion?.SetJumpPower(originalJumpPower);
            locomotion = null;
        }
    }
}
