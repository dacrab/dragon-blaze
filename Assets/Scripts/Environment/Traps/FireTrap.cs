using System;
using System.Threading;
using UnityEngine;
using Core.Constants;
using Core.Managers;
using Core.Services;
using Gameplay.Combat;

namespace Environment.Traps
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public sealed class FireTrap : MonoBehaviour
    {
        [SerializeField] float damage = 10f;
        [SerializeField] float activationDelay = 0.5f;
        [SerializeField] float activeTime = 2f;
        [SerializeField] Color warningColor = Color.red;
        [SerializeField] Color activeColor = Color.white;
        [SerializeField] AudioClip firetrapSound;

        Animator anim;
        SpriteRenderer sprite;
        bool active, activating;
        CancellationTokenSource cycleCts;

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
        }

        void OnDestroy()
        {
            cycleCts?.Cancel();
            cycleCts?.Dispose();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player) || collision.IsInvisiblePlayer()) return;
            if (active || activating) return;
            activating = true;
            cycleCts?.Cancel();
            cycleCts = new CancellationTokenSource();
            _ = ActivateAsync(cycleCts.Token);
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (active && collision.CompareTag(GameConstants.Tags.Player)) collision.DamagePerSecond(damage * Time.deltaTime);
        }

        async Awaitable ActivateAsync(CancellationToken ct)
        {
            sprite.color = warningColor;
            try
            {
                await Awaitable.WaitForSecondsAsync(activationDelay, ct);
                ServiceLocator.Get<IAudioManager>()?.PlaySound(firetrapSound);
                sprite.color = activeColor;
                active = true;
                anim.SetBool(GameConstants.Anim.Activated, true);
                await Awaitable.WaitForSecondsAsync(activeTime, ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            active = false;
            activating = false;
            anim.SetBool(GameConstants.Anim.Activated, false);
        }
    }
}
