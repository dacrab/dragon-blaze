using UnityEngine;

namespace Gameplay.Items.PowerUps
{

public sealed class SpeedBoost : PowerUpBase
{
    [SerializeField] Sprite icon;
    [SerializeField] float multiplier = 2f;

    protected override void Activate(Characters.Player.Player player)
    {
        player.ModifySpeed(multiplier);
        ShowIndicator("Speed Boost", icon);
    }

    protected override void Deactivate(Characters.Player.Player player) => player.ModifySpeed(1f / multiplier);
}
}