using UnityEngine;
using Gameplay.Characters.Player;

public class Invisibility : PowerUpBase
{
    [SerializeField] private Sprite invisibilityImage;

    protected override void ActivatePowerUp(PlayerPowerups playerPowerups)
    {
        SetPlayerVisibility(playerPowerups, false);
        ActivateUIIndicator();
    }

    protected override void DeactivatePowerUp(PlayerPowerups playerPowerups)
    {
        SetPlayerVisibility(playerPowerups, true);
    }

    private void SetPlayerVisibility(PlayerPowerups playerPowerups, bool isVisible)
    {
        PlayerController controller = playerPowerups.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetInvisibility(!isVisible);
        }
    }

    private void ActivateUIIndicator()
    {
        ActivateIndicator("Invisibility", invisibilityImage);
    }
}
