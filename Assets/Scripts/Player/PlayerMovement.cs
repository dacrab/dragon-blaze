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
    [SerializeField] private int extraJumps;
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
    private bool isInvisible = false;
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

    public void setInteracting(bool interacting)
    {
        isInteracting = interacting;
        anim.SetBool("Idle" , isInteracting);
        //If interacting, stop the animation
        anim.SetBool("run" , !isInteracting);
    }

    public void AddScore(int value) //Reference to the UIManager class
    {
        score += value;
        Debug.Log($"Score added: {score}, firing event.");
        OnScoreChanged?.Invoke(score);
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
        if (visible)
        {
            // Reset player visibility to normal
            playerSpriteRenderer.color = Color.white;
        }
        else
        {
            // Set player visibility to invisible
            playerSpriteRenderer.color = invisibleColor;
        }
        isInvisible = !visible;
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
        if (Input.GetKeyDown(KeyCode.Space))
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
            coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
            jumpCounter = extraJumps; //Reset jump counter to extra jump value
            wallJumpCounter = maxWallJumps; // Reset the wall jump counter when grounded
        }
        else
            coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
    }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !IsOnWall() && jumpCounter <= 0) return;
        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);

        // Instantiate jump particles
        if (jumpParticlesPrefab != null)
        {
            Instantiate(jumpParticlesPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Jump Particle System Prefab is not assigned.");
        }

        float horizontalVelocity = body.velocity.x; // Preserve the current horizontal velocity

        if (IsOnWall())
            WallJump();
        else
        {
            if (IsGrounded() || coyoteCounter > 0)
            {
                body.velocity = new Vector2(horizontalVelocity, jumpPower); // Use preserved horizontal velocity
            }
            else
            {
                if (jumpCounter > 0) // If we have extra jumps then jump and decrease the jump counter
                {
                    body.velocity = new Vector2(horizontalVelocity, jumpPower); // Use preserved horizontal velocity
                    jumpCounter--;
                }
            }
        }

        // Reset coyote counter to 0 to avoid double jumps
        coyoteCounter = 0;
    }

    public void SpeedPowerUp(float value)
    {
        speed += value;
    }

    private void WallJump()
    {
        if (wallJumpCounter > 0 && IsOnWall())
        {
            // Perform a wall jump
            int originalLayer = gameObject.layer;
            int nonCollidingLayer = LayerMask.NameToLayer("NonCollidingWithPlatforms");

            if (nonCollidingLayer == -1)
            {
                Debug.LogError("NonCollidingWithPlatforms layer does not exist. Please create it in the Layer Manager.");
                return;
            }

            gameObject.layer = nonCollidingLayer;
            body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
            wallJumpCounter--; // Decrement the wall jump counter

            StartCoroutine(RestoreLayer(originalLayer));
        }
        else if (IsOnWall())
        {
            // Start sliding down the wall
            StartWallSlide();
        }
    }

    private void StartWallSlide()
    {
        // Adjust these values as needed for the desired sliding effect
        body.velocity = new Vector2(0, -1f); // Slow downward movement
        body.gravityScale = 0.5f; // Reduced gravity for sliding
    }

    private IEnumerator RestoreLayer(int originalLayer)
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.layer = originalLayer;
    }

    private bool IsOnWall()
    {
        // Use only the Ground layer for checking wall collisions
        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, mask);
        return raycastHit.collider != null;
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
}
