using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    // Delegate for handling camera translation events
    public delegate void ParallaxCameraDelegate(float deltaMovement);

    // Event triggered when the camera translates
    public ParallaxCameraDelegate onCameraTranslate;

    // Store the old position of the camera
    private float oldPosition;

    void Start()
    {
        // Initialize oldPosition with the initial position of the camera
        oldPosition = transform.position.x;
    }

    void Update()
    {
        // Check if the camera has moved horizontally
        if (transform.position.x != oldPosition)
        {
            // Trigger the camera translation event with the delta movement
            if (onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }

            // Update the old position to the current position
            oldPosition = transform.position.x;
        }
    }
}
