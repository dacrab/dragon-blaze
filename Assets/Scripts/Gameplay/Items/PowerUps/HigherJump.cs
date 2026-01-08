using UnityEngine;

namespace Gameplay.Items.PowerUps;

public sealed class HigherJump : PowerUpBase
{
    [SerializeField] Sprite icon;
    [SerializeField] float multiplier = 1.5f;

    protected override void Activate(Characters.Player.Player player)
    {
        player.ModifyJump(multiplier);
        ShowIndicator("Higher Jump", icon);
    }

    protected override void Deactivate(Characters.Player.Player player) => player.ModifyJump(1f / multiplier);
}
