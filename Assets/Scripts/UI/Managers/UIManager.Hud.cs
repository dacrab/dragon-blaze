using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;

namespace UI.Managers
{
    public sealed partial class UIManager
    {
        void UpdateCoinDisplay(ScoreChangedEvent e)
        {
            if (coinText != null) coinText.SetText("{0}", e.Score);
        }

        void ActivateIndicator(PowerUpActivatedEvent e)
        {
            if (indicatorPrefab == null) return;
            var panel = ResolveIndicatorPanel();
            if (panel == null) return;
            if (indicators.TryGetValue(e.Name, out var existing))
            {
                indicators.Remove(e.Name);
                if (existing != null) Destroy(existing);
            }

            var indicator = Instantiate(indicatorPrefab, panel);
            if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = e.Icon;
            if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = e.Name;
            indicators[e.Name] = indicator;

            _ = FadeOutAsync(e.Name, indicator, e.Duration);
        }

        /// <summary>
        /// Indicators need a persistent container: a serialized panel usually belongs to a per-scene canvas that
        /// is destroyed on load, while this manager persists. Build one under this object's canvas if needed.
        /// </summary>
        Transform? ResolveIndicatorPanel()
        {
            if (indicatorPanel != null) return indicatorPanel;
            if (indicatorPanelHost == null)
            {
                var go = new GameObject("PowerUpIndicators", typeof(RectTransform));
                go.layer = gameObject.layer;
                // Parent to the manager's canvas so UI-space anchoring works; fall back to this transform.
                var canvas = GetComponentInParent<Canvas>();
                go.transform.SetParent(canvas != null ? canvas.transform : transform, false);
                indicatorPanelHost = (RectTransform)go.transform;
            }
            return indicatorPanelHost;
        }

        async Awaitable FadeOutAsync(string name, GameObject indicator, float duration)
        {
            var img = indicator.GetComponentInChildren<Image>();
            float startAlpha = img != null ? img.color.a : 1f;
            float elapsed = 0f;

            // Unscaled so indicators clear while paused/game over instead of lingering forever.
            while (indicator != null && elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (img != null) img.color = new(img.color.r, img.color.g, img.color.b, Mathf.Lerp(startAlpha, 0, elapsed / duration));
                await Awaitable.NextFrameAsync();
            }

            // A re-activation already replaced this entry; leave the fresh indicator alone.
            if (indicator == null) return;
            if (indicators.TryGetValue(name, out var current) && current == indicator) indicators.Remove(name);
            Destroy(indicator);
        }
    }
}
