using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;
    private Transform cameraTransform;
    private Vector3 lastcameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastcameraPosition = cameraTransform.position;
        Sprite sprite =GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY  = texture.height /  sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastcameraPosition;
        transform.position += new Vector3 (deltaMovement.x * parallaxEffectMultiplier.x , deltaMovement.y * parallaxEffectMultiplier.y);
        lastcameraPosition = cameraTransform.position;
        
        //Horizontal Parallax
        if (infiniteHorizontal)
        {
         if (Mathf.Abs (cameraTransform.position.x  - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX  =  (cameraTransform.position.x -  transform.position.x) % textureUnitSizeX;
                transform.position =  new Vector3(cameraTransform.position.x + offsetPositionX , transform.position.y);
            }
        }


        //Vertical Parallax
        if (infiniteVertical)
        {
            if (Mathf.Abs (cameraTransform.position.y  - transform.position.y) >= textureUnitSizeY)
                {
                    float offsetPositionY  =  (cameraTransform.position.y -  transform.position.y) % textureUnitSizeY;
                    transform.position =  new Vector3( transform.position.x ,cameraTransform.position.y + offsetPositionY );
                }
        }
    }
}
