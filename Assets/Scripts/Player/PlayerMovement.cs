using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour 
{
    //=========INVISIBILITY========
    [Header("Invisibility PowerUp")]
    [SerializeField] private float defaultInvisibilityDuration = 5f;
    [SerializeField] private Color invisibleColor = new Color(1f, 1f, 1f, 0.5f); // Change the alpha channel to reduce visibility
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    //=====HIGHER JUMP=======
    [Header("Higher Jump PowerUp")]
    [SerializeField] private float defaultJumpMultiplier = 1.5f;

    //======SPEED BOOST======
    [Header("Speed Boost PowerUp")]
    [SerializeField] private float defaultSpeedBoostMultiplier = 1.5f;
    [SerializeField] private float defaultSpeedBoostDuration = 5f;

    //====MOVEMENT======
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

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

    //========FALLING PARAMETERS=======
    [Header("Falling Parameters")]
    [SerializeField] private float maxFallingTime = 2f; //Maximum time allowed for falling
    private float fallingTimer = 0f; //Track falling time
    private UIManager uiManagerInstance; //Reference to the UIManager for the Game Over 
    private bool gameOverTriggered = false; //Flag to track if the GameOver has been triggered
    private Rigidbody2D rb;


    public void AddScore(int value) //Reference to the UIManager class
    {
        score += value;
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

    public void ApplyInvisibility(float duration)
    {
        StartCoroutine(InvisibilityCoroutine(duration));
    }

    private IEnumerator InvisibilityCoroutine(float duration)
    {
        SetInvisibility(false);
        yield return new WaitForSeconds(duration);
        SetInvisibility(true);
    }

    private void SetVisibility(bool visible)
    {
        if (visible)
        {
            playerSpriteRenderer.color = Color.white;
        }
        else
        {
            playerSpriteRenderer.color = invisibleColor;
        }
        isInvisible = !visible;
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

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
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

        //Set animator parameters
        anim.SetBool("run", Mathf.Abs(horizontalInput) > 0.01f);
        anim.SetBool("grounded", IsGrounded());

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        //Adjustable jump height
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (IsOnWall())
        {
            body.gravityScale = 2;
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (IsGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        }
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !IsOnWall() && jumpCounter <= 0) return;
        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);

        if (IsOnWall())
            WallJump();
        else
        {
            if (IsGrounded())
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            else
            {
                //If not on the ground and coyote counter bigger than 0 do a normal jump
                if (coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                else
                {
                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }

            //Reset coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    public void SpeedPowerUp(float value)
    {
        speed += value;
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        wallJumpCooldown = 0;
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool IsOnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    public bool canAttack()
    {
        return Mathf.Approximately(horizontalInput, 0) && IsGrounded() && !IsOnWall();
    }
}