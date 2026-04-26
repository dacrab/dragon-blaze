using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core.Managers;

namespace UI.HUD
{
    public sealed class PowerUpIndicatorManager : SingletonManager<PowerUpIndicatorManager>
    {
        [SerializeField] GameObject indicatorPrefab;
        [SerializeField] Transform panel;

        readonly Dictionary<string, (GameObject obj, Coroutine routine)> indicators = new();

        public void ActivateIndicator(string name, Sprite icon, float duration)
        {
            if (indicatorPrefab == null || panel == null) return;

            if (indicators.TryGetValue(name, out var existing))
            {
                if (existing.routine != null) StopCoroutine(existing.routine);
                var newRoutine = StartCoroutine(FadeOut(name, existing.obj, duration));
                indicators[name] = (existing.obj, newRoutine);
                return;
            }

            var indicator = Instantiate(indicatorPrefab, panel);
            if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = icon;
            if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = name;

            var routine = StartCoroutine(FadeOut(name, indicator, duration));
            indicators[name] = (indicator, routine);
        }

        IEnumerator FadeOut(string name, GameObject indicator, float duration)
        {
            var img = indicator.GetComponentInChildren<Image>();
            var startAlpha = img != null ? img.color.a : 1f;
            
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                if (img != null) img.color = new(img.color.r, img.color.g, img.color.b, Mathf.Lerp(startAlpha, 0, t / duration));
                yield return null;
            }
            
            indicators.Remove(name);
            Destroy(indicator);
        }
    }
}
}