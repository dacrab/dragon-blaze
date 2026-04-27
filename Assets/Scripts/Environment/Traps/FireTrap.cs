using UnityEngine;
using Core.Managers;
using Core.Constants;

namespace Environment.Traps
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public sealed class FireTrap : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] float damage = 10f;
        
        [Header("Timing")]
        [SerializeField] float activationDelay = 0.5f;
        [SerializeField] float activeTime = 2f;
        
        [Header("Colors")]
        [SerializeField] Color warningColor = Color.red;
        [SerializeField] Color activeColor = Color.white;
        
        [Header("Audio")]
        [SerializeField] AudioClip firetrapSound;

        Animator anim;
        SpriteRenderer sprite;
        bool active;
        Gameplay.Characters.Player.Player cachedPlayer;
        Gameplay.Combat.Health cachedHealth;

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            cachedPlayer = collision.GetComponent<Gameplay.Characters.Player.Player>();
            cachedHealth = collision.GetComponent<Gameplay.Combat.Health>();
            if (cachedPlayer is { IsInvisible: true }) return;
            if (!active) StartCoroutine(Activate());
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (!active || !collision.CompareTag(GameConstants.Tags.Player)) return;
            if (cachedPlayer is { IsInvisible: true }) return;
            cachedHealth?.TakeDamage(damage * Time.deltaTime);
        }

        IEnumerator Activate()
        {
            sprite.color = warningColor;
            yield return new WaitForSeconds(activationDelay);
            
            GameManager.Instance?.PlaySound(firetrapSound);
            sprite.color = activeColor;
            active = true;
            anim.SetBool(GameConstants.Animation.Activated, true);
            
            yield return new WaitForSeconds(activeTime);
            
            active = false;
            anim.SetBool(GameConstants.Animation.Activated, false);
        }
    }
}
}