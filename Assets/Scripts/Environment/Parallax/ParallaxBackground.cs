using UnityEngine;
using UnityEngine.InputSystem;
using Core.Utilities;

namespace Environment.Parallax
{
    /// <summary>
    /// Parallax background effect that follows camera movement.
    /// Uses Unity's Input System for mouse-based parallax in menus.
    /// </summary>
    public class ParallaxBackground : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Vector2 parallaxEffectMultiplier;
        [SerializeField] private bool infiniteHorizontal;
        [SerializeField] private bool infiniteVertical;
        [SerializeField] private bool followMouse;
        [SerializeField] private float mouseSmoothTime = 0.3f;
        #endregion

        #region Private Fields
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Vector2 startPosition;
        private Vector3 velocity;
        #endregion

        #region Unity Lifecycle Methods
        void Start()
        {
            AutoWireHelper.WireAllFields(this);
            
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
            if (followMouse) UpdateMouseParallax();
            else if (cameraTransform != null)
            {
                ApplyParallaxEffect();
                HandleInfiniteScrolling();
            }
        }

        private void InitializeComponents()
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null) return;
            lastCameraPosition = cameraTransform.position;
            // spriteRenderer is auto-wired via [AutoWire]
        }

        private void SetupTextureSize()
        {
            if (spriteRenderer == null || spriteRenderer.sprite == null) return;
            var texture = spriteRenderer.sprite.texture;
            textureUnitSizeX = texture.width / spriteRenderer.sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / spriteRenderer.sprite.pixelsPerUnit;
        }

        private void ApplyParallaxEffect()
        {
            var deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;
        }

        private void HandleInfiniteScrolling()
        {
            if (spriteRenderer == null) return;
            if (infiniteHorizontal) AdjustHorizontalPosition();
            if (infiniteVertical) AdjustVerticalPosition();
        }

        private void AdjustHorizontalPosition()
        {
            float offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            if (Mathf.Abs(offsetX) >= textureUnitSizeX / 2)
                transform.position = new Vector3(cameraTransform.position.x - offsetX, transform.position.y);
        }

        private void AdjustVerticalPosition()
        {
            float offsetY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            if (Mathf.Abs(offsetY) >= textureUnitSizeY / 2)
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y - offsetY);
        }
        
        private void UpdateMouseParallax()
        {
            if (Camera.main == null) return;
            
            // Use Unity's Input System for mouse position
            var mousePosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            var offset = Camera.main.ScreenToViewportPoint(mousePosition);
            var targetPosition = (Vector3)startPosition + new Vector3(offset.x * parallaxEffectMultiplier.x, offset.y * parallaxEffectMultiplier.y, 0f);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, mouseSmoothTime);
        }
        #endregion
    }
}
