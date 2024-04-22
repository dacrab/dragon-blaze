using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    // Factor by which the layer should parallax
    public float parallaxFactor;

    // Move the layer based on the camera's delta movement
    public void Move(float delta)
    {
        // Calculate the new position based on the delta movement and parallax factor
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;

        // Update the local position of the layer
        transform.localPosition = newPos;
    }
}
