using UnityEngine;
using Gameplay.Characters.Player;

namespace Gameplay.Items.PowerUps
{
    public class HigherJump : PowerUpBase
    {
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

        protected override void DeactivatePowerUp(PlayerPowerups playerPowerups) => locomotion?.SetJumpPower(originalJumpPower);
    }
}
