using UnityEngine;

namespace Gameplay.Characters.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "DragonBlaze/Player/Player Config")]
    public class PlayerConfigSO : ScriptableObject
    {
        [Header("Movement")]
        public float speed = 10f;
        public float wallSlideSpeed = 0.3f;
        
        [Header("Jumping")]
        public float jumpPower = 15f;
        public float wallJumpForce = 15f;
        public float wallJumpTime = 0.2f;
        public int extraJumps = 2;
        public float coyoteTime = 0.2f;

        [Header("Dash")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        
        [Header("Physics Checks")]
        public LayerMask groundLayer;
        public float groundCheckExtraHeight = 0.1f;
    }
}
