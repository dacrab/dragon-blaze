using UnityEngine;
using Core.Managers;
using System.Collections;
using Core.Constants;

namespace Environment.Traps;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public sealed class FireTrap : TrapBase
{
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
    Gameplay.Health.Health currentTarget;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (active && currentTarget is { IsAlive: true })
            currentTarget.TakeDamage(damage * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(GameConstants.Tags.Player)) return;
        var player = collision.GetComponent<Gameplay.Characters.Player.Player>();
        if (player is { IsInvisible: true }) return;

        currentTarget = collision.GetComponent<Gameplay.Health.Health>();
        if (!active) StartCoroutine(Activate());
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.Tags.Player)) currentTarget = null;
    }

    IEnumerator Activate()
    {
        sprite.color = warningColor;
        yield return new WaitForSeconds(activationDelay);
        
        SoundManager.Instance?.PlaySound(firetrapSound);
        sprite.color = activeColor;
        active = true;
        anim.SetBool(GameConstants.Animation.Activated, true);
        
        yield return new WaitForSeconds(activeTime);
        
        active = false;
        anim.SetBool(GameConstants.Animation.Activated, false);
    }
}
