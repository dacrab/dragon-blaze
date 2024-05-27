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
        if (indicatorPrefab == null || indicatorsPanel == null)
        {
            Debug.LogError("Setup error: indicatorPrefab or indicatorsPanel is not set.");
            return;
        }

        // Check if an indicator for the same power-up already exists
        GameObject existingIndicator = FindIndicatorByName(powerUpName);
        if (existingIndicator != null)
        {
            // Reset the timer of the existing indicator
            Image existingImage = existingIndicator.GetComponentInChildren<Image>();
            if (existingImage != null)
            {
                existingImage.color = new Color(existingImage.color.r, existingImage.color.g, existingImage.color.b, 1f);
            }
            return;
        }

        // Create a new indicator
        GameObject newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
        
        // Use TMP_Text instead of Text
        TMP_Text textComponent = newIndicator.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = powerUpName;
        }
        else
        {
            Debug.LogError("Text component is missing in the IndicatorPrefab.");
            return;
        }

        Image imageComponent = newIndicator.transform.Find("Image").GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = powerUpImage;
        }
        else
        {
            Debug.LogError("Image component is missing or named incorrectly in the IndicatorPrefab.");
            return;
        }

        StartCoroutine(UpdateIndicator(newIndicator, duration, imageComponent));
        activeIndicators.Add(newIndicator);
        UpdateIndicatorPositions();
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
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            float alpha = remainingTime / duration;
            if (imageComponent != null)
            {
                Color c = imageComponent.color;
                c.a = alpha;
                imageComponent.color = c;
            }
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Once the duration ends, remove and destroy the indicator
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
