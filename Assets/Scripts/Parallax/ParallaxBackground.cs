using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;
    #endregion

    #region Private Fields
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Unity Lifecycle Methods
    void Start()
    {
        InitializeComponents();
        SetupTextureSize();
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        ApplyParallaxEffect();
        HandleInfiniteScrolling();
    }
    #endregion

    #region Private Methods
    private void InitializeComponents()
    {
        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Camera.main is not found. ParallaxBackground requires a main camera.");
            return;
        }

        lastCameraPosition = cameraTransform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object.");
            return;
        }
    }

    private void SetupTextureSize()
    {
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
    #endregion
}
