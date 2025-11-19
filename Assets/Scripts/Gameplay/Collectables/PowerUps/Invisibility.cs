using UnityEngine;

public class Invisibility : PowerUpBase
{
    [SerializeField] private Sprite invisibilityImage;

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        SetPlayerVisibility(playerMovement, false);
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        SetPlayerVisibility(playerMovement, true);
    }

    private void SetPlayerVisibility(PlayerMovement playerMovement, bool isVisible)
    {
        playerMovement.SetInvisibility(!isVisible);
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Invisibility", invisibilityImage);
    }
}
