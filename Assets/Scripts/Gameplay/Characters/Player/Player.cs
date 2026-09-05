using System;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Input;
using Core.Managers;
using Core.Services;
using Core.State;
using Core.Pooling;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class Player : MonoBehaviour, IPlayer
    {
        [Header("Config")]
        [SerializeField] PlayerConfigSO config;

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

        float horizontalInput, coyoteTimer, attackTimer, wallJumpTimer;
        int jumpCount, fireballIndex, groundedId, runId;
        bool facingRight = true, dashing, wallSliding, interacting;
        Transform checkpoint;
        readonly PlayerStats stats = new();
        InputReader inputReader;
        CancellationTokenSource dashCts, invisibilityCts;

        public bool IsInvisible { get; private set; }
        public bool IsGrounded { get; private set; }
        Transform IPlayer.Transform => transform;
        public float Damage => config.baseDamage * stats.Factor(PlayerStat.Damage);
        float Speed => config.speed * stats.Factor(PlayerStat.Speed);
        float JumpPower => config.jumpPower * stats.Factor(PlayerStat.Jump);

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            box = GetComponent<BoxCollider2D>();
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
            groundedId = Animator.StringToHash(GameConstants.Anim.Grounded);
            runId = Animator.StringToHash(GameConstants.Anim.Run);
            ServiceLocator.Register<IPlayer>(this);
        }

        void OnDestroy()
        {
            if (ReferenceEquals(ServiceLocator.Get<IPlayer>(), this)) ServiceLocator.Unregister<IPlayer>();
        }

        void OnEnable()
        {
            inputReader ??= InputReader.Instance;
            if (inputReader != null)
            {
                inputReader.MoveEvent += OnMove;
                inputReader.JumpEvent += OnJump;
                inputReader.JumpCanceledEvent += OnJumpCanceled;
                inputReader.DashEvent += OnDash;
                inputReader.AttackEvent += OnAttack;
            }
            EventBus.Subscribe<DialogueStateChangedEvent>(OnDialogueChanged);
            EventBus.Subscribe<PlayerDiedEvent>(OnDeath);
            EventBus.Subscribe<PlayerRespawnEvent>(OnRespawn);
        }

        void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.MoveEvent -= OnMove;
                inputReader.JumpEvent -= OnJump;
                inputReader.JumpCanceledEvent -= OnJumpCanceled;
                inputReader.DashEvent -= OnDash;
                inputReader.AttackEvent -= OnAttack;
            }
            dashCts?.Cancel();
            invisibilityCts?.Cancel();
            EventBus.Unsubscribe<DialogueStateChangedEvent>(OnDialogueChanged);
            EventBus.Unsubscribe<PlayerDiedEvent>(OnDeath);
            EventBus.Unsubscribe<PlayerRespawnEvent>(OnRespawn);
        }

        void Update()
        {
            attackTimer += Time.deltaTime;
            if (wallJumpTimer > 0) wallJumpTimer -= Time.deltaTime;
            stats.Tick(Time.deltaTime);
            UpdateGroundCheck();
            UpdateWallSlide();
            SetAnimBool(groundedId, IsGrounded);
            SetAnimBool(runId, Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold);
            if (IsGrounded) { coyoteTimer = config.coyoteTime; jumpCount = config.extraJumps; }
            else coyoteTimer -= Time.deltaTime;
        }

        void FixedUpdate()
        {
            if (interacting || dashing || wallJumpTimer > 0) return;
            rb.linearVelocity = new(horizontalInput * Speed, rb.linearVelocity.y);
            if ((horizontalInput > config.movementThreshold && !facingRight) ||
                (horizontalInput < -config.movementThreshold && facingRight)) Flip();
        }

        void OnMove(float x) => horizontalInput = interacting ? 0 : x;
        void OnDialogueChanged(DialogueStateChangedEvent e) => interacting = e.Open;

        void OnJump()
        {
            if (interacting) return;
            if (wallSliding) { WallJump(); return; }
            bool canCoyote = coyoteTimer > 0;
            if (!IsGrounded && !canCoyote && jumpCount <= 0) return;
            rb.linearVelocity = new(rb.linearVelocity.x, JumpPower);
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
            dashing = true;
            float gravity = rb.gravityScale;
            rb.gravityScale = 0;
            rb.linearVelocity = new(transform.localScale.x * config.dashSpeed, 0);
            SpawnVfx(dashVfx);
            dashCts?.Cancel();
            dashCts = new CancellationTokenSource();
            _ = EndDashAsync(gravity, dashCts.Token);
        }

        async Awaitable EndDashAsync(float gravity, CancellationToken ct)
        {
            try
            {
                await Awaitable.WaitForSecondsAsync(config.dashDuration, ct);
                rb.gravityScale = gravity;
                dashing = false;
            }
            catch (OperationCanceledException) { }
        }

        void OnAttack()
        {
            if (attackTimer < config.attackCooldown) return;
            if (Mathf.Abs(rb.linearVelocity.x) > config.velocityThreshold || !IsGrounded) return;
            if (!GameStateManager.IsCurrentlyPlaying) return;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(fireballSound);
            anim.SetTrigger(GameConstants.Anim.Attack);
            attackTimer = 0;
            if (firePoint == null) return;
            ProjectileBase.Fire(fireballs, ref fireballIndex, firePoint.position)?
                .SetDirection(Mathf.Sign(transform.localScale.x));
        }

        void WallJump()
        {
            wallSliding = false;
            wallJumpTimer = config.wallJumpTime;
            jumpCount = config.extraJumps;
            float dir = -transform.localScale.x;
            rb.linearVelocity = new(dir * config.wallJumpForce, JumpPower);
            Flip();
            SpawnVfx(jumpVfx);
        }

        void Flip()
        {
            facingRight = !facingRight;
            var s = transform.localScale;
            transform.localScale = new(-s.x, s.y, s.z);
        }

        void UpdateGroundCheck()
        {
            var hit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down,
                config.groundCheckExtraHeight, config.groundLayer);
            IsGrounded = hit.collider != null;
        }

        void UpdateWallSlide()
        {
            bool shouldCheck = !IsGrounded && Mathf.Abs(horizontalInput) > config.velocityThreshold;
            if (shouldCheck)
            {
                float dir = transform.localScale.x;
                var start = (Vector2)transform.position + new Vector2(dir * config.wallCheckOffsetX, config.wallCheckOffsetY);
                var hit = Physics2D.Linecast(start, start + Vector2.down * config.wallCheckLength, config.groundLayer);
                wallSliding = hit.collider != null;
                if (wallSliding)
                    rb.linearVelocity = new(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -config.wallSlideSpeed));
            }
            else wallSliding = false;

            if (wallSlideParticles != null)
            {
                if (wallSliding && !wallSlideParticles.isPlaying) wallSlideParticles.Play();
                else if (!wallSliding && wallSlideParticles.isPlaying) wallSlideParticles.Stop();
            }
        }

        void SetAnimBool(int id, bool value)
        {
            if (anim.GetBool(id) != value) anim.SetBool(id, value);
        }

        void OnDeath(PlayerDiedEvent _)
        {
            SpawnVfx(deathVfx);
            anim.SetTrigger(GameConstants.Anim.Die);
        }

        void OnRespawn(PlayerRespawnEvent _)
        {
            if (checkpoint == null) return;
            transform.position = checkpoint.position;
            stats.Clear();
            dashCts?.Cancel();
            invisibilityCts?.Cancel();
            SetInvisibility(false);
            jumpCount = config.extraJumps;
            coyoteTimer = 0;
            dashing = false;
            wallSliding = false;
            interacting = false;
            anim.SetTrigger(GameConstants.Anim.Respawn);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag(GameConstants.Tags.Checkpoint)) return;
            checkpoint = col.transform;
            ServiceLocator.Get<IAudioManager>()?.PlaySound(checkpointSound);
            col.enabled = false;
            col.GetComponent<Animator>()?.SetTrigger(GameConstants.Anim.Activate);
        }

        public void SetInvisibility(bool invisible)
        {
            IsInvisible = invisible;
            sprite.color = invisible ? config.invisibleColor : Color.white;
        }

        public void SetInvisibilityFor(float duration)
        {
            invisibilityCts?.Cancel();
            invisibilityCts = new CancellationTokenSource();
            _ = InvisibilityTimeoutAsync(duration, invisibilityCts.Token);
        }

        public bool HasCheckpoint() => checkpoint != null;

        async Awaitable InvisibilityTimeoutAsync(float duration, CancellationToken ct)
        {
            try
            {
                SetInvisibility(true);
                await Awaitable.WaitForSecondsAsync(duration, ct);
                SetInvisibility(false);
            }
            catch (OperationCanceledException) { }
        }

        public void AddModifier(PlayerStat stat, float factor, float duration) => stats.Add(stat, factor, duration);

        void SpawnVfx(GameObject prefab) => VfxPool.Spawn(prefab, transform.position, Quaternion.identity);
    }
}
