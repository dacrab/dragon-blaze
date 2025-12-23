using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core.Utilities;

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
        if (indicatorPrefab == null || indicatorsPanel == null) return;

        var existing = activeIndicators.Find(ind => 
            ind.GetComponentInChildren<TMP_Text>()?.text.Contains(powerUpName) == true);
        
        if (existing != null)
        {
            existing.SetActive(true);
            var img = existing.GetComponentInChildren<Image>();
            StopCoroutine(UpdateIndicator(existing, duration, img));
            StartCoroutine(UpdateIndicator(existing, duration, img));
            return;
        }

        var newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
        if (newIndicator == null) return;

        var imageComponent = newIndicator.transform.Find("Image")?.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = powerUpImage;
            imageComponent.color = imageComponent.color.WithAlpha(INITIAL_OPACITY);
        }

        var textComponent = newIndicator.GetComponentInChildren<TMP_Text>();
        textComponent?.SetText($"<b><size=120%>{powerUpName}</size></b>");

        StartCoroutine(UpdateIndicator(newIndicator, duration, imageComponent));
        activeIndicators.Add(newIndicator);
        UpdateIndicatorPositions();
    }

    private IEnumerator UpdateIndicator(GameObject indicator, float duration, Image imageComponent)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            if (imageComponent != null) imageComponent.color = imageComponent.color.WithAlpha(remainingTime / duration);
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
