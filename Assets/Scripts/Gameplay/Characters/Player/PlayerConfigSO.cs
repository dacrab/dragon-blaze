using UnityEngine;

namespace Gameplay.Characters.Player
{
    // WARNING: Do NOT modify these values at runtime. ScriptableObjects are shared assets.
    // Runtime modifications will persist across play sessions and affect all instances.
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "DragonBlaze/Player/Player Config")]
    public sealed class PlayerConfigSO : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _wallSlideSpeed = 0.3f;
        [SerializeField] private float _movementThreshold = 0.01f;
        [SerializeField] private float _velocityThreshold = 0.1f;
        
        [Header("Jumping")]
        [SerializeField] private float _jumpPower = 15f;
        [SerializeField] private float _wallJumpForce = 15f;
        [SerializeField] private float _wallJumpTime = 0.2f;
        [SerializeField] private int _extraJumps = 2;
        [SerializeField] private float _coyoteTime = 0.2f;
        [SerializeField] private float _jumpCancelMultiplier = 0.5f;

        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 20f;
        [SerializeField] private float _dashDuration = 0.2f;
        
        [Header("Wall Detection")]
        [SerializeField] private float _wallCheckOffsetX = 0.4f;
        [SerializeField] private float _wallCheckOffsetY = 0.2f;
        [SerializeField] private float _wallCheckLength = 0.8f;
        
        [Header("Ground Detection")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _groundCheckExtraHeight = 0.1f;

        [Header("Combat")]
        [SerializeField] private float _attackCooldown = 0.5f;
        [SerializeField] private float _baseDamage = 10f;

        [Header("Visuals")]
        [SerializeField] private Color _invisibleColor = new(1, 1, 1, 0.5f);

        public float Speed => _speed;
        public float WallSlideSpeed => _wallSlideSpeed;
        public float MovementThreshold => _movementThreshold;
        public float VelocityThreshold => _velocityThreshold;
        public float JumpPower => _jumpPower;
        public float WallJumpForce => _wallJumpForce;
        public float WallJumpTime => _wallJumpTime;
        public int ExtraJumps => _extraJumps;
        public float CoyoteTime => _coyoteTime;
        public float JumpCancelMultiplier => _jumpCancelMultiplier;
        public float DashSpeed => _dashSpeed;
        public float DashDuration => _dashDuration;
        public float WallCheckOffsetX => _wallCheckOffsetX;
        public float WallCheckOffsetY => _wallCheckOffsetY;
        public float WallCheckLength => _wallCheckLength;
        public LayerMask GroundLayer => _groundLayer;
        public float GroundCheckExtraHeight => _groundCheckExtraHeight;
        public float AttackCooldown => _attackCooldown;
        public float BaseDamage => _baseDamage;
        public Color InvisibleColor => _invisibleColor;
    }
}