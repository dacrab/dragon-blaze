using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Add this

public class PowerUpIndicatorManager : MonoBehaviour
{
    public GameObject indicatorPrefab; // Prefab for the indicator
    public Transform indicatorsPanel; // Parent panel for indicators

    private List<GameObject> activeIndicators = new List<GameObject>(); // List to keep track of active indicators

    public void ActivateIndicator(string powerUpName, Sprite powerUpImage, float duration)
    {
        if (indicatorPrefab == null) {
            Debug.LogError("indicatorPrefab is not set.");
            return;
        }
        if (indicatorsPanel == null) {
            Debug.LogError("indicatorsPanel is not set.");
            return;
        }

        GameObject newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
        if (newIndicator == null) {
            Debug.LogError("Failed to instantiate newIndicator.");
            return;
        }

        Image imageComponent = newIndicator.transform.Find("Image").GetComponent<Image>();
        if (imageComponent == null) {
            Debug.LogError("Image component not found in newIndicator.");
            return;
        }

        imageComponent.sprite = powerUpImage;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0.5f); // Set to 50% opacity

        StartCoroutine(UpdateIndicator(newIndicator, duration, imageComponent));
        activeIndicators.Add(newIndicator);
        UpdateIndicatorPositions();

        // Set the name of the power-up
        TMP_Text textComponent = newIndicator.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            // Apply bold and increase size
            textComponent.text = $"<b><size=120%>{powerUpName}</size></b>";
        }
        else
        {
            Debug.LogError("Text component not found in newIndicator.");
        }
    }

    private GameObject FindIndicatorByName(string powerUpName)
    {
        foreach (GameObject indicator in activeIndicators)
        {
            TMP_Text textComponent = indicator.GetComponentInChildren<TMP_Text>();
            if (textComponent != null && textComponent.text == powerUpName)
            {
                return indicator;
            }
        }
        return null;
    }

    private IEnumerator UpdateIndicator(GameObject indicator, float duration, Image imageComponent)
    {
        // Fade out the image over the duration
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            if (imageComponent != null)
            {
                float alpha = remainingTime / duration;
                imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, alpha);
            }
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Once the duration ends, fade the image back in
        while (imageComponent.color.a < 1.0f)
        {
            float alpha = imageComponent.color.a + Time.deltaTime / duration; // Adjust fade-in time as needed
            imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, Mathf.Min(alpha, 1.0f));
            yield return null;
        }

        // Optionally, remove and destroy the indicator if not needed anymore
        activeIndicators.Remove(indicator);
        Destroy(indicator);
        UpdateIndicatorPositions();
    }

    private void UpdateIndicatorPositions() {
        float currentPositionX = 0;
        float maxHeight = 0;
        int gap = 10; // Gap in pixels between indicators

        foreach (GameObject indicator in activeIndicators) {
            RectTransform rect = indicator.GetComponent<RectTransform>();
            TMP_Text textComponent = indicator.GetComponentInChildren<TMP_Text>();

            // Calculate the required width and height
            float indicatorWidth = Mathf.Min(LayoutUtility.GetPreferredWidth(textComponent.rectTransform), 200); // Max width of 200
            float indicatorHeight = LayoutUtility.GetPreferredHeight(textComponent.rectTransform);

            // Set the position
            rect.localPosition = new Vector3(currentPositionX, 0, 0);

            // Update the currentPositionX for the next indicator
            currentPositionX += indicatorWidth + gap;

            // Track the maximum height
            if (indicatorHeight > maxHeight) {
                maxHeight = indicatorHeight;
            }
        }

        // Optionally adjust the height of the container if you want all indicators to align vertically
        foreach (GameObject indicator in activeIndicators) {
            RectTransform rect = indicator.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, maxHeight);
        }
    }
}
