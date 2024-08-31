using UnityEngine;

public class HigherJump : PowerUpBase
{
    [SerializeField] private float jumpMultiplier = 1.5f;
    [SerializeField] private Sprite higherJumpImage;

    private float originalJumpPower;

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        StoreOriginalJumpPower(playerMovement);
        ApplyJumpMultiplier(playerMovement);
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        ResetJumpPower(playerMovement);
    }

    private void StoreOriginalJumpPower(PlayerMovement playerMovement)
    {
        originalJumpPower = playerMovement.jumpPower;
    }

    private void ApplyJumpMultiplier(PlayerMovement playerMovement)
    {
        playerMovement.jumpPower = originalJumpPower * jumpMultiplier;
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Higher Jump", higherJumpImage);
    }

    private void ResetJumpPower(PlayerMovement playerMovement)
    {
        playerMovement.jumpPower = originalJumpPower;
    }
}
