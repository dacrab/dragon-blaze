using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private AudioClip checkpoint;
    #endregion

    #region Private Fields
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeComponents();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCheckpointCollision(collision);
    }
    #endregion

    #region Public Methods
    public void RespawnCheck()
    {
        if (currentCheckpoint == null)
        {
            uiManager.GameOver();
            return;
        }

        RespawnPlayer();
    }

    public Transform GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindObjectOfType<UIManager>();
    }

    private void HandleCheckpointCollision(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            SetCheckpoint(collision);
            ActivateCheckpoint(collision);
        }
    }

    private void SetCheckpoint(Collider2D checkpoint)
    {
        currentCheckpoint = checkpoint.transform;
        SoundManager.instance.PlaySound(this.checkpoint);
    }

    private void ActivateCheckpoint(Collider2D checkpoint)
    {
        checkpoint.enabled = false;
        checkpoint.GetComponent<Animator>().SetTrigger("activate");
    }

    private void RespawnPlayer()
    {
        playerHealth.Respawn();
        transform.position = currentCheckpoint.position;
    }
    #endregion
}