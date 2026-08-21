using UnityEngine;
using Core.Services;

namespace Core.Constants
{
    public static class GameConstants
    {
        public static class Tags
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string Checkpoint = "Checkpoint";
        }

        public static class Anim
        {
            public const string Grounded = "grounded";
            public const string Run = "run";
            public const string Die = "die";
            public const string Respawn = "respawn";
            public const string MeleeAttack = "meleeAttack";
            public const string RangedAttack = "rangedAttack";
            public const string Attack = "attack";
            public const string Moving = "moving";
            public const string Hurt = "hurt";
            public const string Activated = "activated";
            public const string Activate = "activate";
            public const string Explode = "explode";
        }

        public static class Layers
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
        }

        public static Transform FindPlayer() => ServiceLocator.Get<IPlayer>()?.Transform;
    }
}
