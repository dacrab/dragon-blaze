using UnityEngine;
using Core.Managers;
using Core.Constants;
using Gameplay.Characters.Player;
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
        bool active, cycling;
        Color originalColor;
        Player cachedPlayer;
        Health cachedHealth;

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
            originalColor = sprite.color;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            collision.TryGetComponent(out cachedPlayer);
            collision.TryGetComponent(out cachedHealth);
            if (cachedPlayer is { IsInvisible: true }) return;
            if (!cycling) _ = ActivateAsync();
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            if (cachedPlayer is { IsInvisible: true }) return;
            if (active)
            {
                cachedHealth?.TakeDamage(damage * Time.deltaTime);
                return;
            }
            if (!cycling) _ = ActivateAsync();
        }

        async Awaitable ActivateAsync()
        {
            cycling = true;
            sprite.color = warningColor;
            await Awaitable.WaitForSecondsAsync(activationDelay);
            GameManager.Instance?.PlaySound(firetrapSound);
            sprite.color = activeColor;
            active = true;
            anim.SetBool(GameConstants.Anim.Activated, true);
            await Awaitable.WaitForSecondsAsync(activeTime);
            active = false;
            anim.SetBool(GameConstants.Anim.Activated, false);
            sprite.color = originalColor;
            cycling = false;
        }
    }
}
