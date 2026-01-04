using UnityEngine;
using UnityEngine.InputSystem;

namespace Environment.Parallax
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxEffectMultiplier;
        [SerializeField] private bool infiniteHorizontal;
        [SerializeField] private bool infiniteVertical;
        [SerializeField] private bool followMouse;
        [SerializeField] private float mouseSmoothTime = 0.3f;

        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureUnitSizeX;
        private float textureUnitSizeY;
        private SpriteRenderer spriteRenderer;
        private Vector2 startPosition;
        private Vector3 velocity;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (!followMouse)
            {
                cameraTransform = Camera.main?.transform;
                if (cameraTransform != null) lastCameraPosition = cameraTransform.position;
                
                if (spriteRenderer != null && spriteRenderer.sprite != null)
                {
                    var texture = spriteRenderer.sprite.texture;
                    textureUnitSizeX = texture.width / spriteRenderer.sprite.pixelsPerUnit;
                    textureUnitSizeY = texture.height / spriteRenderer.sprite.pixelsPerUnit;
                }
            }
            else startPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (followMouse) UpdateMouseParallax();
            else if (cameraTransform != null)
            {
                var delta = cameraTransform.position - lastCameraPosition;
                transform.position += new Vector3(delta.x * parallaxEffectMultiplier.x, delta.y * parallaxEffectMultiplier.y);
                lastCameraPosition = cameraTransform.position;

                if (spriteRenderer != null)
                {
                    if (infiniteHorizontal)
                    {
                        float offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                        if (Mathf.Abs(offsetX) >= textureUnitSizeX / 2)
                            transform.position = new Vector3(cameraTransform.position.x - offsetX, transform.position.y);
                    }
                    if (infiniteVertical)
                    {
                        float offsetY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                        if (Mathf.Abs(offsetY) >= textureUnitSizeY / 2)
                            transform.position = new Vector3(transform.position.x, cameraTransform.position.y - offsetY);
                    }
                }
            }
        }
        
        private void UpdateMouseParallax()
        {
            if (Camera.main == null) return;
            var mousePos = Mouse.current?.position.ReadValue() ?? Vector2.zero;
            var offset = Camera.main.ScreenToViewportPoint(mousePos);
            var target = (Vector3)startPosition + new Vector3(offset.x * parallaxEffectMultiplier.x, offset.y * parallaxEffectMultiplier.y, 0f);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, mouseSmoothTime);
        }
    }
}
