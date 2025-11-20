using UnityEngine;
using Gameplay.Characters.Player;

public class HigherJump : PowerUpBase
{
    [SerializeField] private float jumpMultiplier = 1.5f;
    [SerializeField] private Sprite higherJumpImage;

    private float originalJumpPower;

    protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
    {
        PlayerLocomotion locomotion = playerPowerups.GetComponent<PlayerLocomotion>();
        if (locomotion != null)
        {
            StoreOriginalJumpPower(locomotion);
            ApplyJumpMultiplier(locomotion);
        }
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
    {
        PlayerLocomotion locomotion = playerPowerups.GetComponent<PlayerLocomotion>();
        if (locomotion != null)
        {
            ResetJumpPower(locomotion);
        }
    }

    private void StoreOriginalJumpPower(PlayerLocomotion locomotion)
    {
        originalJumpPower = locomotion.GetJumpPower();
    }

    private void ApplyJumpMultiplier(PlayerLocomotion locomotion)
    {
        locomotion.SetJumpPower(originalJumpPower * jumpMultiplier);
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Higher Jump", higherJumpImage);
    }

    private void ResetJumpPower(PlayerLocomotion locomotion)
    {
        locomotion.SetJumpPower(originalJumpPower);
    }
}
