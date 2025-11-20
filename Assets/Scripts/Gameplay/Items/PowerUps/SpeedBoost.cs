using UnityEngine;
using Gameplay.Characters.Player;

public class SpeedBoost : PowerUpBase
{
    [SerializeField] private Sprite speedBoostImage;
    [SerializeField] private float speedMultiplier = 2f;

    protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
    {
        playerPowerups.ApplySpeedBoost(speedMultiplier, duration);
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
    {
        // Logic handled by PlayerPowerups internal coroutine, but we need to satisfy abstract method.
        // Or we change PowerUpBase to not enforce Deactivate if PlayerPowerups handles it.
        // But PowerUpBase assumes it handles the timer. 
        // PlayerPowerups.ApplySpeedBoost also starts a coroutine.
        // This is double timer.
        
        // Best approach: PlayerPowerups exposes method "SetSpeedMultiplier(val)" and we handle timer here?
        // OR PlayerPowerups handles timer, and we just trigger it.
        
        // In PlayerPowerups refactor, I added ApplySpeedBoost(multiplier, duration).
        // So we just call that in Activate.
        // Deactivate can be empty or removed from abstract if we refactor base.
        // But since base is abstract, we must implement.
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Speed Boost", speedBoostImage);
    }
}
