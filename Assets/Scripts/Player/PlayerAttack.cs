using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs;
    [SerializeField] private AudioClip fireballSound;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown && playerMovement.canAttack() && Time.timeScale > 0)
        {
            Attack();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        if (SoundManager.instance == null)
        {
            Debug.LogError("SoundManager instance is not initialized.");
            return;
        }
        if (fireballs == null || fireballs.Length == 0)
        {
            Debug.LogError("Fireballs array is not initialized or empty.");
            return;
        }
        if (firePoint == null)
        {
            Debug.LogError("FirePoint is not assigned.");
            return;
        }

        SoundManager.instance.PlaySound(fireballSound);
        anim.SetTrigger("attack");
        cooldownTimer = 0;

        int fireballIndex = FindFireball();
        if (fireballIndex != -1 && fireballs[fireballIndex] != null)
        {
            fireballs[fireballIndex].transform.position = firePoint.position;
            fireballs[fireballIndex].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        }
        else
        {
            Debug.LogError("Invalid fireball index or fireball is null.");
        }
    }

    private int FindFireball()
    {
        for (int i = 0; i < fireballs.Length; i++)
        {
            if (!fireballs[i].activeInHierarchy)
                return i;
        }
        return -1;
    }
}
