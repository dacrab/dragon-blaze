using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Gameplay.Items.PowerUps
{
    public class PowerUpIndicatorManager : MonoBehaviour
    {
        [Header("References")]
        public GameObject indicatorPrefab;
        public Transform indicatorsPanel;

        private const int GAP = 10;
        private const float MAX_WIDTH = 200f;
        private List<GameObject> activeIndicators = new List<GameObject>();

        public void ActivateIndicator(string powerUpName, Sprite powerUpImage, float duration)
        {
            if (indicatorPrefab == null || indicatorsPanel == null) return;

            var existing = activeIndicators.Find(ind => 
                ind.GetComponentInChildren<TMP_Text>()?.text.Contains(powerUpName) == true);
            
            if (existing != null)
            {
                existing.SetActive(true);
                StartCoroutine(UpdateIndicator(existing, duration, existing.GetComponentInChildren<Image>()));
                return;
            }

            var newIndicator = Instantiate(indicatorPrefab, indicatorsPanel);
            var img = newIndicator.transform.Find("Image")?.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = powerUpImage;
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
            }

            var text = newIndicator.GetComponentInChildren<TMP_Text>();
            text?.SetText($"<b><size=120%>{powerUpName}</size></b>");

            StartCoroutine(UpdateIndicator(newIndicator, duration, img));
            activeIndicators.Add(newIndicator);
            UpdatePositions();
        }

        private IEnumerator UpdateIndicator(GameObject indicator, float duration, Image img)
        {
            float remaining = duration;
            while (remaining > 0)
            {
                if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, remaining / duration);
                remaining -= Time.deltaTime;
                yield return null;
            }
            activeIndicators.Remove(indicator);
            Destroy(indicator);
            UpdatePositions();
        }

        private void UpdatePositions()
        {
            float x = 0;
            foreach (var ind in activeIndicators)
            {
                var rect = ind.GetComponent<RectTransform>();
                var text = ind.GetComponentInChildren<TMP_Text>();
                float width = Mathf.Min(LayoutUtility.GetPreferredWidth(text.rectTransform), MAX_WIDTH);
                rect.localPosition = new Vector3(x, 0, 0);
                x += width + GAP;
            }
        }
    }
}
