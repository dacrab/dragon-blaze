using UnityEngine;

namespace Gameplay.Items.PowerUps
{

public sealed class Invisibility : PowerUpBase
{
    [SerializeField] Sprite icon;

    protected override void Activate(Characters.Player.Player player)
    {
        player.SetInvisibility(true);
        ShowIndicator("Invisibility", icon);
    }

    protected override void Deactivate(Characters.Player.Player player) => player.SetInvisibility(false);
}
}