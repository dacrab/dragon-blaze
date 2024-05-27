using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invisibility : PowerUpBase
{
    [SerializeField] private Sprite invisibilityImage; // Assign this in the inspector

    protected override void ActivatePowerUp(PlayerMovement playerMovement)
    {
        // Make the player invisible and immune
        playerMovement.SetVisibility(false);
        ActivateIndicator("Invisibility", invisibilityImage);
    }

    protected override void DeactivatePowerUp(PlayerMovement playerMovement)
    {
        // Make the player visible and vulnerable again
        playerMovement.SetVisibility(true);
    }
}
