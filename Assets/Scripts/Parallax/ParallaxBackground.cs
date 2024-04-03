using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>(); // List to store parallax layers

    void Start()
    {
        // If parallaxCamera is not assigned, try to find it from the main camera
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        // Subscribe to the onCameraTranslate event of the parallaxCamera
        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;

        // Set up the parallax layers
        SetLayers();
    }

    // Populate the parallaxLayers list with ParallaxLayer components from child objects
    void SetLayers()
    {
        parallaxLayers.Clear(); // Clear the list before populating

        // Iterate through child objects and add ParallaxLayer components to the list
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i; // Rename the layer for clarity
                parallaxLayers.Add(layer); // Add the layer to the list
            }
        }
    }

    // Move all parallax layers by the given delta
    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta); // Move each layer by the given delta
        }
    }
}
