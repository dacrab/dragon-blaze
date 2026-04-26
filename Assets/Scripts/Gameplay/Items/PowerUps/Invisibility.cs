using UnityEngine;

namespace Gameplay.Items.PowerUps
{
    public sealed class Invisibility : PowerUpBase
    {
        protected override void Activate(Characters.Player.Player player) => player.SetInvisibility(true);
        protected override void Deactivate(Characters.Player.Player player) => player.SetInvisibility(false);
    }
}
}