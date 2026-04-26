using UnityEngine;

namespace Gameplay.Items.PowerUps
{
    public sealed class HigherJump : PowerUpBase
    {
        [SerializeField] float multiplier = 1.5f;

        protected override void Activate(Characters.Player.Player player) => player.ModifyJump(multiplier);
        protected override void Deactivate(Characters.Player.Player player) => player.ModifyJump(1f / multiplier);
    }
}
}