using UnityEngine;

public class HigherJump : PowerUpBase
{
    [SerializeField] private float jumpMultiplier = 1.5f; // Multiplier to enhance the jump
    [SerializeField] private Sprite higherJumpImage; // Assign this in the inspector
    private float originalJumpPower; // To store the original jump power

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        // Store the original jump power
        originalJumpPower = playerMovement.GetJumpPower();
        // Apply the jump multiplier
        playerMovement.SetJumpPower(originalJumpPower * jumpMultiplier);
        // Activate the UI indicator
        ActivateIndicator("Higher Jump", higherJumpImage);
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        // Reset the jump power back to the original
        playerMovement.SetJumpPower(originalJumpPower);
    }
}
