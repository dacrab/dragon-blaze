using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PowerUpIndicatorManager : MonoBehaviour
{
    [Header("References")]
    public GameObject indicatorPrefab;
    public Transform indicatorsPanel;

    [Header("Settings")]
    private const int GAP = 10;
    private const float MAX_INDICATOR_WIDTH = 200f;
    private const float INITIAL_OPACITY = 0.5f;

    private List<GameObject> activeIndicators = new List<GameObject>();

    public void ActivateIndicator(string powerUpName, Sprite powerUpImage, float duration)
    {
        if (!ValidateReferences()) return;

        GameObject existingIndicator = FindIndicatorByName(powerUpName);
        if (existingIndicator != null)
        {
            ResetExistingIndicator(existingIndicator, duration);
            return;
        }

        CreateNewIndicator(powerUpName, powerUpImage, duration);
    }

    private bool ValidateReferences()
    {
        if (indicatorPrefab == null)
        {
            Debug.LogError("indicatorPrefab is not set.");
            return false;
        }
        if (indicatorsPanel == null)
        {
            Debug.LogError("indicatorsPanel is not set.");
            return false;
        }
        return true;
    }

    private void ResetExistingIndicator(GameObject indicator, float duration)
    {
        indicator.SetActive(true);
        Image imageComponent = indicator.GetComponentInChildren<Image>();
        StopCoroutine(UpdateIndicator(indicator, duration, imageComponent));
        StartCoroutine(UpdateIndicator(indicator, duration, imageComponent));
    }

    private void CreateNewIndicator(string powerUpName, Sprite powerUpImage, float duration)
    {
        GameObject newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
        if (newIndicator == null)
        {
            Debug.LogError("Failed to instantiate newIndicator.");
            return;
        }

        SetupIndicatorComponents(newIndicator, powerUpName, powerUpImage, duration);
        activeIndicators.Add(newIndicator);
        UpdateIndicatorPositions();
    }

    private void SetupIndicatorComponents(GameObject indicator, string powerUpName, Sprite powerUpImage, float duration)
    {
        Image imageComponent = indicator.transform.Find("Image").GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("Image component not found in indicator.");
            return;
        }

        imageComponent.sprite = powerUpImage;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, INITIAL_OPACITY);

        TMP_Text textComponent = indicator.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = $"<b><size=120%>{powerUpName}</size></b>";
        }
        else
        {
            Debug.LogError("Text component not found in indicator.");
        }

        StartCoroutine(UpdateIndicator(indicator, duration, imageComponent));
    }

    private GameObject FindIndicatorByName(string powerUpName)
    {
        return activeIndicators.Find(indicator => 
        {
            TMP_Text textComponent = indicator.GetComponentInChildren<TMP_Text>();
            return textComponent != null && textComponent.text.Contains(powerUpName);
        });
    }

    private IEnumerator UpdateIndicator(GameObject indicator, float duration, Image imageComponent)
    {
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

        activeIndicators.Remove(indicator);
        Destroy(indicator);
        UpdateIndicatorPositions();
    }

    private void UpdateIndicatorPositions()
    {
        float currentPositionX = 0;
        float maxHeight = 0;

        foreach (GameObject indicator in activeIndicators)
        {
            RectTransform rect = indicator.GetComponent<RectTransform>();
            TMP_Text textComponent = indicator.GetComponentInChildren<TMP_Text>();

            float indicatorWidth = Mathf.Min(LayoutUtility.GetPreferredWidth(textComponent.rectTransform), MAX_INDICATOR_WIDTH);
            float indicatorHeight = LayoutUtility.GetPreferredHeight(textComponent.rectTransform);

            rect.localPosition = new Vector3(currentPositionX, 0, 0);
            currentPositionX += indicatorWidth + GAP;
            maxHeight = Mathf.Max(maxHeight, indicatorHeight);
        }

        foreach (GameObject indicator in activeIndicators)
        {
            RectTransform rect = indicator.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, maxHeight);
        }
    }
}
