using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    //=========INVISIBILITY========
    [Header("Invisibility PowerUp")]
    [SerializeField] public float defaultInvisibilityDuration = 5f;
    [SerializeField] public Color invisibleColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] public SpriteRenderer playerSpriteRenderer;
    private bool isInvisible = false;

    //=====HIGHER JUMP=======
    [Header("Higher Jump PowerUp")]
    [SerializeField] private float defaultJumpMultiplier = 1.5f;

    //======SPEED BOOST======
    [Header("Speed Boost PowerUp")]
    [SerializeField] private float defaultSpeedBoostMultiplier = 1.5f;
    [SerializeField] private float defaultSpeedBoostDuration = 5f;

    //====MOVEMENT======
    [Header("Movement Parameters")]
    [SerializeField] public float jumpPower;
    [SerializeField] public float speed;

    //======COYOTE=======
    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    //=====MULTIPLE JUMPS======
    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps = 2;
    private int jumpCounter;

    //=======LAYERS=======
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    //=========SOUNDS==========
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    //========SCORE/COINS=========
    [Header("Coins")]
    private int score = 0;
    public static event Action<int> OnScoreChanged;

    //========FALLING PARAMETERS=======
    [Header("Falling Parameters")]
    [SerializeField] private float maxFallingTime = 2f;
    private float fallingTimer = 0f;
    private UIManager uiManagerInstance;
    private bool gameOverTriggered = false;

    //===========INTERACTIONS=========
    private bool isInteracting;

    // Added for particle system
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject jumpParticlesPrefab;

    // Added for dash
    [Header("Dash Effects")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private GameObject dashParticlesPrefab;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing;
    private float dashTimer;

    // Added for wall slide
    [Header("Wall Slide")]
    private Transform wallCheck; // Added this line
    [SerializeField]private float wallCheckDistance = 0.5f;
    private bool isWallSliding = false;
    [SerializeField] private float wallSlideSpeed = 2f; // Adjust the speed value as needed

    // Added for wall jump
    [Header("Wall Jump")]
    [SerializeField] private int maxWallJumps = 3;
    private int wallJumpCount = 0;

    // Newly added declarations
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;

    private void Awake()
    {
        uiManagerInstance = FindObjectOfType<UIManager>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        wallCheck = transform.Find("WallCheck"); // Ensure there's a child object named 'WallCheck' positioned appropriately
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleFalling();
        HandleMovement();
        HandleJump();
        HandleDash();
        CheckWallSliding();
        HandleWallJump();
    }

    private void HandleFalling()
    {
        if (!IsGrounded())
        {
            fallingTimer += Time.deltaTime;

            if (fallingTimer > maxFallingTime && !gameOverTriggered)
            {
                Die();
                gameOverTriggered = true;
            }
        }
        else
        {
            fallingTimer = 0f;
            gameOverTriggered = false;
        }
    }

    private void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isInteracting)
        {
            anim.SetBool("grounded", true);
        }
        else
        {
            anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        }

        anim.SetBool("grounded", IsGrounded());

        body.gravityScale = 7; // Ensure gravity is always applied
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
    }

    private void HandleJump()
    {
        if ((IsGrounded() || coyoteCounter > 0 || jumpCounter > 0) && Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (IsGrounded())
        {
            ResetJumpState();
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
    }

    private void ResetJumpState()
    {
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
            jumpCounter = extraJumps;
            body.gravityScale = 1;
        }
    }

    private void Jump()
    {
        if (IsGrounded() || coyoteCounter > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            jumpCounter = extraJumps;
            Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);  // Play jump sound
        }
        else if (jumpCounter > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            jumpCounter--;
            Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);  // Play jump sound
        }
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.1f;
        // Assuming "Ground" is the name of the layer you want to check against
        int groundLayerMask = LayerMask.GetMask("Ground"); 
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, groundLayerMask);
        return raycastHit.collider != null;
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }

    public bool IsVisible()
    {
        return !isInvisible;
    }

    public void SetInvisibility(bool visible)
    {
        isInvisible = !visible;
        playerSpriteRenderer.color = visible ? Color.white : invisibleColor;
    }

    public void ApplyInvisibility(float? duration = null)
    {
        StartCoroutine(InvisibilityCoroutine(duration ?? defaultInvisibilityDuration));
    }

    private IEnumerator InvisibilityCoroutine(float duration)
    {
        SetInvisibility(false);
        yield return new WaitForSeconds(duration);
        SetInvisibility(true);
    }

    public void SetVisibility(bool isVisible)
    {
        playerSpriteRenderer.color = isVisible ? Color.white : invisibleColor;
    }

    public void ApplyDefaultHigherJump()
    {
        ApplyHigherJump(defaultJumpMultiplier);
    }

    public void ApplyHigherJump(float multiplier)
    {
        StartCoroutine(HigherJumpCoroutine(multiplier));
    }

    private IEnumerator HigherJumpCoroutine(float multiplier)
    {
        float originalJumpPower = jumpPower;
        jumpPower *= multiplier;
        yield return new WaitForSeconds(defaultSpeedBoostDuration);
        jumpPower = originalJumpPower;
    }

    public void ApplySpeedBoost(float? multiplier = null, float? duration = null)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier ?? defaultSpeedBoostMultiplier, duration ?? defaultSpeedBoostDuration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed *= multiplier;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    public void setInteracting(bool interacting)
    {
        isInteracting = interacting;
        anim.SetBool("grounded", isInteracting);
        anim.SetBool("run", !interacting);

        if (interacting)
        {
            body.velocity = Vector2.zero;
        }
    }

    public void AddScore(int value)
    {
        GameManager.instance.AddCoins(value);
        OnScoreChanged?.Invoke(GameManager.instance.TotalCoins);
    }

    public void Die()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f && !IsGrounded())
            return;

        if (uiManagerInstance != null)
        {
            uiManagerInstance.GameOver();
        }
        else
        {
            Debug.LogWarning("UIManager instance is not found!");
        }

        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Death Particle System Prefab is not assigned.");
        }

        score = 0;
        foreach (Coin coin in FindObjectsOfType<Coin>())
        {
            coin.ResetValue();
        }

        this.enabled = false;
    }

    public int GetScore()
    {
        return score;
    }

    public bool canAttack()
    {
        return Mathf.Approximately(horizontalInput, 0) && IsGrounded();
    }

    public float GetJumpPower()
    {
        return jumpPower;
    }

    public void SetJumpPower(float value)
    {
        jumpPower = value;
    }

    public void TakeDamage(int damage)
    {
        if (isInvisible) return;
    }

    private void HandleDash()
    {
        // Check if the player is moving by checking the horizontal input
        if (Input.GetKeyDown(KeyCode.LeftShift) && Mathf.Abs(horizontalInput) > 0.01f)
        {
            PlayDashEffects();
            if (!isDashing)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void PlayDashEffects()
    {
        // Play dash sound
        if (dashSound != null)
        {
            AudioSource.PlayClipAtPoint(dashSound, transform.position);
        }
        else
        {
            Debug.LogWarning("Dash sound clip is not assigned.");
        }

        // Instantiate dash particles
        if (dashParticlesPrefab != null)
        {
            Instantiate(dashParticlesPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Dash Particle System Prefab is not assigned.");
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float originalSpeed = speed;
        speed = dashSpeed;

        // Lower the opacity
        Color originalColor = playerSpriteRenderer.color;
        playerSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f); // Set alpha to 0.5

        yield return new WaitForSeconds(dashDuration);

        // Reset opacity and speed
        playerSpriteRenderer.color = originalColor;
        speed = originalSpeed;
        isDashing = false;
    }

    private void CheckWallSliding()
    {
        if (wallCheck == null)
        {
            Debug.LogError("WallCheck Transform is not assigned in PlayerMovement script.");
            return;
        }

        // Check for walls on both sides
        RaycastHit2D hitLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, groundLayer);

        if ((hitLeft.collider != null || hitRight.collider != null) && !IsGrounded() && body.velocity.y < 0)
        {
            if (wallJumpCount < maxWallJumps)
            {
                StartWallSlide();
            }
        }
        else
        {
            isWallSliding = false;
            wallJumpCount = 0; // Reset wall jump count when not near a wall
        }

        if (isWallSliding)
        {
            body.velocity = new Vector2(body.velocity.x, -wallSlideSpeed); // Use wallSlideSpeed here
        }
    }

    private void HandleWallJump()
    {
        if (isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            WallJump();
        }
    }

    private void WallJump()
    {
        float jumpDirection = transform.localScale.x > 0 ? -1 : 1; // Jump in the opposite direction of the wall
        body.velocity = new Vector2(jumpPower * jumpDirection, jumpPower);
        wallJumpCount++;
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1); // Flip player to face opposite direction
        Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);  // Play jump sound
        isWallSliding = false; // Exit wall sliding state immediately after jumping
    }

    private void StartWallSlide()
    {
        isWallSliding = true;
        // Optionally play a sound or animate
    }
}
