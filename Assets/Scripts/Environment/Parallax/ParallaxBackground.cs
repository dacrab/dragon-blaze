using UnityEngine;

namespace Environment.Parallax
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] Vector2 parallaxMultiplier;
        [SerializeField] bool infiniteHorizontal, infiniteVertical;
        [SerializeField, Range(0.1f, 1f)] float wrapThreshold = 0.5f;

        Transform cam;
        Vector3 lastCamPos;
        float texSizeX, texSizeY;

        void Start()
        {
            cam = Camera.main?.transform;
            if (cam != null) lastCamPos = cam.position;
            var s = GetComponent<SpriteRenderer>().sprite;
            if (s != null)
            {
                texSizeX = s.texture.width / s.pixelsPerUnit;
                texSizeY = s.texture.height / s.pixelsPerUnit;
            }
        }

        void LateUpdate()
        {
            if (cam == null) return;
            var delta = cam.position - lastCamPos;
            var pos = transform.position + new Vector3(delta.x * parallaxMultiplier.x, delta.y * parallaxMultiplier.y);
            lastCamPos = cam.position;

            if (infiniteHorizontal)
            {
                float ox = (cam.position.x - pos.x) % texSizeX;
                if (Mathf.Abs(ox) >= texSizeX * wrapThreshold) pos.x = cam.position.x - ox;
            }
            if (infiniteVertical)
            {
                float oy = (cam.position.y - pos.y) % texSizeY;
                if (Mathf.Abs(oy) >= texSizeY * wrapThreshold) pos.y = cam.position.y - oy;
            }
            transform.position = pos;
        }
    }
}
