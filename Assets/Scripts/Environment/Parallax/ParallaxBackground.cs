using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;
    [SerializeField] private bool followMouse; // Added for menu compatibility
    [SerializeField] private float mouseSmoothTime = 0.3f;
    #endregion

    #region Private Fields
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private SpriteRenderer spriteRenderer;
    
    // Menu Specific
    private Vector2 startPosition;
    private Vector3 velocity;
    #endregion

    #region Unity Lifecycle Methods
    void Start()
    {
        if (!followMouse)
        {
            InitializeComponents();
            SetupTextureSize();
        }
        else
        {
            startPosition = transform.position;
        }
    }

    private void LateUpdate()
    {
        if (followMouse)
        {
            UpdateMouseParallax();
        }
        else
        {
            if (cameraTransform == null) return;
            ApplyParallaxEffect();
            HandleInfiniteScrolling();
        }
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            // Optional: Debug.LogWarning for menu scenes where this might not matter
            return;
        }

        lastCameraPosition = cameraTransform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetupTextureSize()
    {
        if (spriteRenderer == null) return;
        
        Texture2D texture = spriteRenderer.sprite.texture;
        textureUnitSizeX = texture.width / spriteRenderer.sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / spriteRenderer.sprite.pixelsPerUnit;
    }

    private void ApplyParallaxEffect()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;
    }

    private void HandleInfiniteScrolling()
    {
        if (spriteRenderer == null) return;

        if (infiniteHorizontal)
        {
            AdjustHorizontalPosition();
        }

        if (infiniteVertical)
        {
            AdjustVerticalPosition();
        }
    }

    private void AdjustHorizontalPosition()
    {
        float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
        if (Mathf.Abs(offsetPositionX) >= textureUnitSizeX / 2)
        {
            transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y);
        }
    }

    private void AdjustVerticalPosition()
    {
        float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
        if (Mathf.Abs(offsetPositionY) >= textureUnitSizeY / 2)
        {
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y - offsetPositionY);
        }
    }
    
    private void UpdateMouseParallax()
    {
        if (Camera.main == null) return;
        
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        // Use parallaxEffectMultiplier.x as offset multiplier
        Vector2 targetPosition = startPosition + (offset * parallaxEffectMultiplier.x);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, mouseSmoothTime);
    }
    #endregion
}
