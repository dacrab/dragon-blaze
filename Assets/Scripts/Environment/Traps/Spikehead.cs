using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;

namespace Environment.Traps;

public sealed class Spikehead : TrapBase
{
    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float range = 5f;
    [SerializeField] float checkDelay = 0.5f;
    
    [Header("Detection")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector3[] checkDirections = { Vector3.right, Vector3.left, Vector3.up, Vector3.down };
    
    [Header("Audio")]
    [SerializeField] AudioClip impactSound;

    Vector3 moveDir;
    float checkTimer;
    bool attacking;

    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying) return;

        if (attacking)
            transform.Translate(moveDir * speed * Time.deltaTime);
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkDelay) CheckForPlayer();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.Instance?.PlaySound(impactSound);
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            var player = collision.GetComponent<Gameplay.Characters.Player.Player>();
            if (player is not { IsInvisible: true })
                base.OnTriggerEnter2D(collision);
        }
        Stop();
    }

    void CheckForPlayer()
    {
        foreach (var dir in checkDirections)
        {
            var worldDir = transform.TransformDirection(dir);
            var hit = Physics2D.Raycast(transform.position, worldDir, range, playerLayer);
            if (hit.collider == null) continue;
            
            var player = hit.collider.GetComponent<Gameplay.Characters.Player.Player>();
            if (player is { IsInvisible: true }) continue;
            
            attacking = true;
            moveDir = worldDir;
            checkTimer = 0;
            return;
        }
    }

    void Stop() { moveDir = Vector3.zero; attacking = false; }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var dir in checkDirections)
            Gizmos.DrawRay(transform.position, transform.TransformDirection(dir) * range);
    }
}
