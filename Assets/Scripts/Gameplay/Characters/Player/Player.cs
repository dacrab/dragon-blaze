using System;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Events;
using Core.Input;
using Core.Services;
using Gameplay.Combat;

namespace Gameplay.Characters.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed partial class Player : MonoBehaviour, IPlayer
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
        Vector3? checkpoint;
        Vector3 spawnPosition;
        readonly PlayerStats stats = new();
        InputReader? inputReader;
        CancellationTokenSource? dashCts, invisibilityCts;
        float dashRestoreGravity;

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
            // Remember where the level starts so a respawn without a checkpoint still has somewhere to go.
            spawnPosition = transform.position;
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
            CancelDash();
            CancelInvisibility();
            EventBus.Unsubscribe<DialogueStateChangedEvent>(OnDialogueChanged);
            EventBus.Unsubscribe<PlayerDiedEvent>(OnDeath);
            EventBus.Unsubscribe<PlayerRespawnEvent>(OnRespawn);
        }
    }
}
