using UnityEngine;

namespace Gameplay.Items.PowerUps
{

public sealed class DamageBoost : PowerUpBase
{
    [SerializeField] Sprite icon;
    [SerializeField] float multiplier = 2f;

    float originalDamage;

    protected override void Activate(Characters.Player.Player player)
    {
        originalDamage = player.Damage;
        player.SetDamage(originalDamage * multiplier);
        ShowIndicator("Damage Boost", icon);
    }

    protected override void Deactivate(Characters.Player.Player player) => player.SetDamage(originalDamage);
}
}