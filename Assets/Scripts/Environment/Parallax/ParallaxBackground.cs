using UnityEngine;

namespace Environment.Parallax
{

[RequireComponent(typeof(SpriteRenderer))]
public sealed class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] Vector2 parallaxMultiplier;
    [SerializeField] bool infiniteHorizontal, infiniteVertical;
    
    [Header("Wrapping")]
    [SerializeField, Range(0.1f, 1f)] float wrapThreshold = 0.5f;

    Transform cam;
    Vector3 lastCamPos;
    float textureSizeX, textureSizeY;

    void Start()
    {
        cam = Camera.main?.transform;
        if (cam != null) lastCamPos = cam.position;

        var sprite = GetComponent<SpriteRenderer>().sprite;
        if (sprite != null)
        {
            textureSizeX = sprite.texture.width / sprite.pixelsPerUnit;
            textureSizeY = sprite.texture.height / sprite.pixelsPerUnit;
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;

        var delta = cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxMultiplier.x, delta.y * parallaxMultiplier.y);
        lastCamPos = cam.position;

        if (infiniteHorizontal)
        {
            float offsetX = (cam.position.x - transform.position.x) % textureSizeX;
            if (Mathf.Abs(offsetX) >= textureSizeX * wrapThreshold)
                transform.position = new(cam.position.x - offsetX, transform.position.y);
        }
        if (infiniteVertical)
        {
            float offsetY = (cam.position.y - transform.position.y) % textureSizeY;
            if (Mathf.Abs(offsetY) >= textureSizeY * wrapThreshold)
                transform.position = new(transform.position.x, cam.position.y - offsetY);
        }
    }
}
}