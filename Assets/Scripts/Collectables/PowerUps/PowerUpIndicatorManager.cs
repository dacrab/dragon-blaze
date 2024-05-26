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
            Slider existingSlider = existingIndicator.GetComponentInChildren<Slider>();
            if (existingSlider != null)
            {
                existingSlider.maxValue = duration;
                existingSlider.value = duration;
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

        Slider timeSlider = newIndicator.GetComponentInChildren<Slider>();
        if (timeSlider != null)
        {
            timeSlider.maxValue = duration;
            timeSlider.value = duration;
        }
        else
        {
            Debug.LogError("Slider component is missing in the IndicatorPrefab.");
            return;
        }

        StartCoroutine(UpdateIndicator(newIndicator, duration, timeSlider));
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

    private IEnumerator UpdateIndicator(GameObject indicator, float duration, Slider slider)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            slider.value = remainingTime;
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Once the duration ends, remove and destroy the indicator
        activeIndicators.Remove(indicator);
        Destroy(indicator);
        UpdateIndicatorPositions();
    }

    private void UpdateIndicatorPositions()
    {
        // Update positions so indicators don't overlap
        for (int i = 0; i < activeIndicators.Count; i++)
        {
            activeIndicators[i].transform.localPosition = new Vector3(i * 100, 0, 0); // Adjust spacing as needed
        }
    }
}
