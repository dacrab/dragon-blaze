using UnityEngine;
using Core.Managers;
using Core.Constants;
using Core.State;

namespace Environment.Traps
{

public sealed class Spikehead : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] float damage = 10f;
    
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
    Gameplay.Characters.Player.Player cachedPlayer;
    Gameplay.Combat.Health cachedHealth;

    void Update()
    {
        if (!GameStateManager.IsCurrentlyPlaying) return;

        if (attacking)
            transform.Translate(moveDir * speed * Time.deltaTime);
        else if ((checkTimer += Time.deltaTime) >= checkDelay)
            CheckForPlayer();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance?.PlaySound(impactSound);
        if (collision.CompareTag(GameConstants.Tags.Player))
        {
            cachedPlayer = collision.GetComponent<Gameplay.Characters.Player.Player>();
            cachedHealth = collision.GetComponent<Gameplay.Combat.Health>();
            if (cachedPlayer is not { IsInvisible: true })
                cachedHealth?.TakeDamage(damage);
        }
        
        attacking = false;
        moveDir = Vector3.zero;
    }

    void CheckForPlayer()
    {
        checkTimer = 0;
        foreach (var dir in checkDirections)
        {
            var worldDir = GetWorldDirection(dir);
            var hit = Physics2D.Raycast(transform.position, worldDir, range, playerLayer);
            if (hit.collider != null)
            {
                cachedPlayer = hit.collider.GetComponent<Gameplay.Characters.Player.Player>();
                if (cachedPlayer is { IsInvisible: false })
                {
                    attacking = true;
                    moveDir = worldDir;
                    return;
                }
            }
        }
    }

    Vector3 GetWorldDirection(Vector3 localDir) => transform.TransformDirection(localDir);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (var dir in checkDirections)
            Gizmos.DrawRay(transform.position, GetWorldDirection(dir) * range);
    }
}
}