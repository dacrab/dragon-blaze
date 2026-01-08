using UnityEngine;

namespace Gameplay.Characters.Player;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "DragonBlaze/Player/Player Config")]
public sealed class PlayerConfigSO : ScriptableObject
{
    [Header("Movement")]
    public float speed = 10f;
    public float wallSlideSpeed = 0.3f;
    public float movementThreshold = 0.01f;
    public float velocityThreshold = 0.1f;
    
    [Header("Jumping")]
    public float jumpPower = 15f;
    public float wallJumpForce = 15f;
    public float wallJumpTime = 0.2f;
    public int extraJumps = 2;
    public float coyoteTime = 0.2f;
    public float jumpCancelMultiplier = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    
    [Header("Wall Detection")]
    public float wallCheckOffsetX = 0.4f;
    public float wallCheckOffsetY = 0.2f;
    public float wallCheckLength = 0.8f;
    
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckExtraHeight = 0.1f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    public float baseDamage = 10f;

    [Header("Visuals")]
    public Color invisibleColor = new(1, 1, 1, 0.5f);
}
