using UnityEngine;
using System.Collections;
using Core.Interfaces;
using Core.Input;
using Core.Events;
using Core.Constants;
using Core.Managers;
using Core.State;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class Player : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] PlayerConfigSO config;
    [SerializeField] InputReader input;

    [Header("Combat")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject[] fireballs;
    [SerializeField] AudioClip fireballSound;

    [Header("Effects")]
    [SerializeField] ParticleSystem wallSlideParticles;
    [SerializeField] GameObject jumpVfx, dashVfx, deathVfx;
    [SerializeField] AudioClip checkpointSound;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;
    Health.Health health;

    float horizontalInput, coyoteTimer, attackTimer, wallJumpTimer;
    int jumpCount, fireballIndex;
    bool facingRight = true, dashing, wallSliding, interacting;
    Transform checkpoint;
    float currentDamage;

    public bool IsInvisible { get; private set; }
    public bool IsGrounded { get; private set; }
    public float Damage => currentDamage;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health.Health>();
        currentDamage = config.baseDamage;
    }

    void OnEnable()
    {
        if (input == null) return;
        input.MoveEvent += OnMove;
        input.JumpEvent += OnJump;
        input.JumpCanceledEvent += OnJumpCanceled;
        input.DashEvent += OnDash;
        input.AttackEvent += OnAttack;
        EventBus.OnDialogueStateChanged += b => interacting = b;
        EventBus.OnPlayerDied += OnDeath;
        EventBus.OnPlayerRespawn += OnRespawn;
    }

    void OnDisable()
    {
        if (input == null) return;
        input.MoveEvent -= OnMove;
        input.JumpEvent -= OnJump;
        input.JumpCanceledEvent -= OnJumpCanceled;
        input.DashEvent -= OnDash;
        input.AttackEvent -= OnAttack;
        EventBus.OnDialogueStateChanged -= b => interacting = b;
        EventBus.OnPlayerDied -= OnDeath;
        EventBus.OnPlayerRespawn -= OnRespawn;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (wallJumpTimer > 0) wallJumpTimer -= Time.deltaTime;

        UpdateGroundCheck();
        UpdateWallSlide();
        UpdateAnimations();

        if (IsGrounded) { coyoteTimer = config.coyoteTime; jumpCount = config.extraJumps; }
        else coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (interacting || dashing || wallJumpTimer > 0) return;
        rb.linearVelocity = new(horizontalInput * config.speed, rb.linearVelocity.y);
        if ((horizontalInput > config.movementThreshold && !facingRight) || (horizontalInput < -config.movementThreshold && facingRight)) Flip();
    }

    void OnMove(float x) => horizontalInput = interacting ? 0 : x;

    void OnJump()
    {
        if (interacting) return;
        bool canCoyote = coyoteTimer > 0;
        
        if (wallSliding) { WallJump(); return; }
        if (!IsGrounded && !canCoyote && jumpCount <= 0) return;

        rb.linearVelocity = new(rb.linearVelocity.x, config.jumpPower);
        SpawnVfx(jumpVfx);

        if (!IsGrounded && !canCoyote) jumpCount--;
        if (canCoyote) coyoteTimer = 0;
    }

    void OnJumpCanceled()
    {
        if (rb.linearVelocity.y > 0)
            rb.linearVelocity = new(rb.linearVelocity.x, rb.linearVelocity.y * config.jumpCancelMultiplier);
    }

    void OnDash()
    {
        if (interacting || dashing || Mathf.Abs(horizontalInput) < config.movementThreshold) return;
        StartCoroutine(DashRoutine());
    }

    void OnAttack()
    {
        if (attackTimer < config.attackCooldown) return;
        if (Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold || !IsGrounded) return;
        if (!GameStateManager.IsCurrentlyPlaying) return;

        SoundManager.Instance?.PlaySound(fireballSound);
        anim.SetTrigger(GameConstants.Animation.Attack);
        attackTimer = 0;

        if (firePoint == null || fireballs is not { Length: > 0 }) return;
        var fb = fireballs[fireballIndex];
        fireballIndex = (fireballIndex + 1) % fireballs.Length;
        fb.transform.position = firePoint.position;
        fb.GetComponent<ProjectileBase>()?.SetDirection(Mathf.Sign(transform.localScale.x));
    }

    void WallJump()
    {
        wallSliding = false;
        wallJumpTimer = config.wallJumpTime;
        jumpCount = config.extraJumps;
        float dir = -transform.localScale.x;
        rb.linearVelocity = new(dir * config.wallJumpForce, config.jumpPower);
        Flip();
        SpawnVfx(jumpVfx);
    }

    IEnumerator DashRoutine()
    {
        dashing = true;
        float gravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new(transform.localScale.x * config.dashSpeed, 0);
        SpawnVfx(dashVfx);
        yield return new WaitForSeconds(config.dashDuration);
        rb.gravityScale = gravity;
        dashing = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void UpdateGroundCheck()
    {
        var hit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, config.groundCheckExtraHeight, config.groundLayer);
        IsGrounded = hit.collider != null;
    }

    void UpdateWallSlide()
    {
        float dir = transform.localScale.x;
        var start = (Vector2)transform.position + new Vector2(dir * config.wallCheckOffsetX, config.wallCheckOffsetY);
        var hit = Physics2D.Linecast(start, start + Vector2.down * config.wallCheckLength, config.groundLayer);
        wallSliding = hit.collider != null && !IsGrounded && Mathf.Abs(horizontalInput) > config.velocityThreshold;
        
        if (wallSliding)
            rb.linearVelocity = new(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -config.wallSlideSpeed));

        if (wallSlideParticles != null)
        {
            if (wallSliding && !wallSlideParticles.isPlaying) wallSlideParticles.Play();
            else if (!wallSliding && wallSlideParticles.isPlaying) wallSlideParticles.Stop();
        }
    }

    void UpdateAnimations()
    {
        anim.SetBool(GameConstants.Animation.Grounded, IsGrounded);
        anim.SetBool(GameConstants.Animation.Run, Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold);
    }

    void OnDeath()
    {
        SpawnVfx(deathVfx);
        anim.SetTrigger(GameConstants.Animation.Die);
    }

    void OnRespawn()
    {
        if (checkpoint == null) return;
        transform.position = checkpoint.position;
        anim.SetTrigger(GameConstants.Animation.Respawn);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(GameConstants.Tags.Checkpoint)) return;
        checkpoint = col.transform;
        SoundManager.Instance?.PlaySound(checkpointSound);
        col.enabled = false;
        col.GetComponent<Animator>()?.SetTrigger(GameConstants.Animation.Activate);
    }

    public void SetInvisibility(bool invisible)
    {
        IsInvisible = invisible;
        sprite.color = invisible ? config.invisibleColor : Color.white;
    }

    public void SetDamage(float d) => currentDamage = d;
    public void ModifySpeed(float mult) => config.speed *= mult;
    public void ModifyJump(float mult) => config.jumpPower *= mult;

    void SpawnVfx(GameObject prefab) { if (prefab != null) Instantiate(prefab, transform.position, Quaternion.identity); }
}
}