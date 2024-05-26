using UnityEngine;

public class SpeedBoost : PowerUpBase
{
    [SerializeField] private Sprite speedBoostImage; // Assign this in the inspector

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        playerMovement.speed *= 2;
        ActivateIndicator("Speed Boost", speedBoostImage);
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        playerMovement.speed /= 2;
    }
}
