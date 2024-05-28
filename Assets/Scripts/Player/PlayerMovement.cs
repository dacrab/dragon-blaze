using UnityEngine;
using System.Collections;
using System; // Added for Action delegate


public class PlayerMovement : MonoBehaviour 
{
    //=========INVISIBILITY========
    [Header("Invisibility PowerUp")]
    [SerializeField] public float defaultInvisibilityDuration = 5f;
    [SerializeField] public Color invisibleColor = new Color(1f, 1f, 1f, 0.5f); // Change the alpha channel to reduce visibility
    [SerializeField] public SpriteRenderer playerSpriteRenderer;

    //=====HIGHER JUMP=======
    [Header("Higher Jump PowerUp")]
    [SerializeField] private float defaultJumpMultiplier = 1.5f;

    //======SPEED BOOST======
    [Header("Speed Boost PowerUp")]
    [SerializeField] private float defaultSpeedBoostMultiplier = 1.5f;
    [SerializeField] private float defaultSpeedBoostDuration = 5f;

    //====MOVEMENT======
    [Header("Movement Parameters")]
    [SerializeField] public float jumpPower; // Changed to public
    [SerializeField] public float speed;  // Define the speed variable

    //======COYOTE=======
    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    //=====MULTIPLE JUMPS======
    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps = 2; // Player can jump two additional times in the air
    private int jumpCounter;

    //=====WALL JUMP=========
    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force
    [SerializeField] private int maxWallJumps = 2; // Maximum number of consecutive wall jumps
    private int wallJumpCounter; // Counter for the number of wall jumps performed

    //=======LAYERS=======
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    //=========SOUNDS==========
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    private bool isInvisible = false;  // Keep it private to encapsulate the field

    // Public method to access the invisibility state
    public bool IsInvisible()
    {
        return isInvisible;
    }

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    //========SCORE/COINS=========
    [Header("Coins")]
    private int score = 0; //Players Score
    public static event Action<int> OnScoreChanged;

    //========FALLING PARAMETERS=======
    [Header("Falling Parameters")]
    [SerializeField] private float maxFallingTime = 2f; //Maximum time allowed for falling
    private float fallingTimer = 0f; //Track falling time
    private UIManager uiManagerInstance; //Reference to the UIManager for the Game Over 
    private bool gameOverTriggered = false; //Flag to track if the GameOver has been triggered

    //===========INTERACTIONS=========
    private bool isInteracting;

    // Added for particle system
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject jumpParticlesPrefab; // Added for jump particles

    private GameObject lastWallTouched; // Store the last wall the player touched

    public void setInteracting(bool interacting)
    {
        isInteracting = interacting;
        anim.SetBool("Idle", isInteracting);
        anim.SetBool("run", !isInteracting);

        // Reset velocity when starting an interaction to prevent slipping
        if (interacting)
        {
            body.velocity = Vector2.zero;
        }
    }

    public void AddScore(int value)
    {
        GameManager.instance.AddCoins(value); // Assuming GameManager has a method to handle score updates
        Debug.Log($"Score added: {GameManager.instance.TotalCoins}, firing event.");
        OnScoreChanged?.Invoke(GameManager.instance.TotalCoins);
    }

    public void Die()
    {
        if (uiManagerInstance != null)
        {
            uiManagerInstance.GameOver();
        }
        else
        {
            Debug.LogWarning("UIManager instance is not found!");
        }

        // Instantiate death particles at player's position
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
    }

    public int GetScore()
    {
        return score;
    }

    private void Awake()
    {
        uiManagerInstance =  FindObjectOfType<UIManager>(); //Get the UIManager component

        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        //Gets sprite rendere component of the player
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        wallJumpCounter = maxWallJumps; // Initialize the wall jump counter
    }

    public bool IsVisible()
    {
        return !isInvisible; // Assuming isInvisible indicates whether the player is currently invisible
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
        SetInvisibility(false);  // Player becomes invisible
        yield return new WaitForSeconds(duration);
        SetInvisibility(true);  // Player becomes visible again
    }

    // Change the access modifier to public


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

    private void Update()
    {
        {
            if (!IsGrounded() && !IsOnWall())
        {
            fallingTimer += Time.deltaTime; //Increase the falling timer when not grounded

            if (fallingTimer > maxFallingTime && !gameOverTriggered) //If the falling timer exceed maximum value apply death to player
            {
                Die(); //Trigger Death
                gameOverTriggered = true;
            }
        }
        else
        {
            fallingTimer = 0f; //Reset time when grounded
            gameOverTriggered = false;
        }

        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (isInteracting)
        {
            anim.SetBool("interacting", true);
        }
        else
        {
        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        }

        //Set animator parameters
        anim.SetBool("grounded", IsGrounded());

        //Jump
        if ((IsGrounded() || coyoteCounter > 0 || jumpCounter > 0) && Input.GetKeyDown(KeyCode.Space))
            Jump();

        //Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (IsOnWall() && Mathf.Approximately(horizontalInput, 0))
        {
            // Apply slow fall only when on the wall and no horizontal input
            body.gravityScale = 0.5f; // Reduced gravity scale for slower fall
            body.velocity = new Vector2(0, -1f); // Apply a small downward force, ensuring no horizontal movement
        }
        else
        {
            // Normal movement and gravity
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        }

        if (IsGrounded())
        {
            ResetJumpState();
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Handle input for wall jumping
        if (Input.GetKeyDown(KeyCode.Space) && IsOnWall() && wallJumpCounter > 0)
        {
            WallJump();
        }
    }
    }

    private void ResetJumpState()
    {
        coyoteCounter = coyoteTime;
        jumpCounter = extraJumps;
        wallJumpCounter = maxWallJumps;
        body.gravityScale = 1; // Reset gravity scale to normal
        lastWallTouched = null; // Reset last wall touched
    }

    private void Jump()
    {
        if (IsGrounded() || coyoteCounter > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower); // Normal jump
            jumpCounter = extraJumps; // Reset extra jumps when grounded
        }
        else if (jumpCounter > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower); // Extra jump
            jumpCounter--; // Decrement the jump counter
        }
    }

    private void WallJump()
    {
        if (IsOnWall() && wallJumpCounter > 0)
        {
            // Apply a force to jump off the wall
            float jumpDirection = Mathf.Sign(transform.localScale.x) * -1; // Jump in the opposite direction of the wall
            body.velocity = new Vector2(wallJumpX * jumpDirection, wallJumpY);
            wallJumpCounter--; // Decrement the wall jump counter

            if (wallJumpCounter == 0)
            {
                StartWallSlide(); // Start sliding down if no wall jumps left
            }
        }
    }

    private void StartWallSlide()
    {
        body.gravityScale = 0.5f; // Reduced gravity for slower fall
        body.velocity = new Vector2(0, -1f); // Slow downward movement
    }

    private bool IsOnWall()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.1f, wallLayer);
        if (hit.collider != null)
        {
            if (lastWallTouched != hit.collider.gameObject)
            {
                lastWallTouched = hit.collider.gameObject;
                wallJumpCounter = maxWallJumps; // Reset wall jump counter when a new wall is touched
                return true; // New wall touched
            }
        }
        return false; // No new wall or same wall
    }

    private bool IsOnAnotherWall()
    {
        // Logic to determine if the player is on a different wall segment
        // This might involve checking the player's position against the last wall touched
        // For simplicity, you might treat any wall contact as a new wall if desired
        return IsOnWall();
    }

    public bool canAttack()
    {
        return Mathf.Approximately(horizontalInput, 0) && IsGrounded() && !IsOnWall();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.1f;
        // Combine the masks for Ground and NonCollidingWithPlatforms layers
        int combinedLayerMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("NonCollidingWithPlatforms");

        // Declare raycastHit only once
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, extraHeight, combinedLayerMask);
        return raycastHit.collider != null;
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
        if (isInvisible) return;  // Ignore damage if invisible

        // Regular damage processing here
    }
}
