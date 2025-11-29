using UnityEngine;
using UnityEngine.InputSystem;

namespace Environment.Parallax
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxMultiplier;
        [SerializeField] private bool infiniteHorizontal;
        [SerializeField] private bool infiniteVertical;
        [SerializeField] private bool followMouse;
        [SerializeField] private float mouseSmoothTime = 0.3f;

        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        private float textureSizeX;
        private float textureSizeY;
        private SpriteRenderer spriteRenderer;
        private Vector2 startPosition;
        private Vector3 velocity;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (followMouse)
            {
                startPosition = transform.position;
            }
            else
            {
                cameraTransform = Camera.main?.transform;
                if (cameraTransform != null)
                {
                    lastCameraPosition = cameraTransform.position;
                    var texture = spriteRenderer.sprite.texture;
                    var ppu = spriteRenderer.sprite.pixelsPerUnit;
                    textureSizeX = texture.width / ppu;
                    textureSizeY = texture.height / ppu;
                }
            }
        }

        private void LateUpdate()
        {
            if (followMouse)
            {
                UpdateMouseParallax();
            }
            else if (cameraTransform != null)
            {
                ApplyParallaxEffect();
            }
        }

        private void ApplyParallaxEffect()
        {
            Vector3 delta = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(delta.x * parallaxMultiplier.x, delta.y * parallaxMultiplier.y);
            lastCameraPosition = cameraTransform.position;

            if (infiniteHorizontal)
            {
                float offsetX = (cameraTransform.position.x - transform.position.x) % textureSizeX;
                if (Mathf.Abs(offsetX) >= textureSizeX / 2)
                    transform.position = new Vector3(cameraTransform.position.x - offsetX, transform.position.y);
            }

            if (infiniteVertical)
            {
                float offsetY = (cameraTransform.position.y - transform.position.y) % textureSizeY;
                if (Mathf.Abs(offsetY) >= textureSizeY / 2)
                    transform.position = new Vector3(transform.position.x, cameraTransform.position.y - offsetY);
            }
        }

        private void UpdateMouseParallax()
        {
            if (Camera.main == null || Mouse.current == null) return;
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 offset = Camera.main.ScreenToViewportPoint(mousePos);
            Vector2 target = startPosition + (offset * parallaxMultiplier.x);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, mouseSmoothTime);
        }
    }
}
