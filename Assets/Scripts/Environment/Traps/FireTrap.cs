using UnityEngine;
using Core.Managers;
using System.Collections;
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

        void Awake()
        {
            anim = GetComponent<Animator>();
            sprite = GetComponent<SpriteRenderer>();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.Player>();
            if (player is { IsInvisible: true }) return;
            if (!active) StartCoroutine(Activate());
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (!active || !collision.CompareTag(GameConstants.Tags.Player)) return;
            var player = collision.GetComponent<Gameplay.Characters.Player.Player>();
            if (player is { IsInvisible: true }) return;
            collision.GetComponent<Gameplay.Combat.Health>()?.TakeDamage(damage * Time.deltaTime);
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