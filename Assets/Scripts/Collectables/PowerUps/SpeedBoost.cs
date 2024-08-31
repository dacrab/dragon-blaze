using UnityEngine;

public class SpeedBoost : PowerUpBase
{
    [SerializeField] private Sprite speedBoostImage;
    [SerializeField] private float speedMultiplier = 2f;

    private float originalSpeed;

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        StoreOriginalSpeed(playerMovement);
        ApplySpeedBoost(playerMovement);
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        ResetSpeed(playerMovement);
    }

    private void StoreOriginalSpeed(PlayerMovement playerMovement)
    {
        originalSpeed = playerMovement.speed;
    }

    private void ApplySpeedBoost(PlayerMovement playerMovement)
    {
        playerMovement.speed *= speedMultiplier;
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Speed Boost", speedBoostImage);
    }

    private void ResetSpeed(PlayerMovement playerMovement)
    {
        playerMovement.speed = originalSpeed;
    }
}
