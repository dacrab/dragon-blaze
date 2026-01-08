using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core.Managers;

namespace Gameplay.Items.PowerUps;

public sealed class PowerUpIndicatorManager : SingletonManager<PowerUpIndicatorManager>
{
    [SerializeField] GameObject indicatorPrefab;
    [SerializeField] Transform panel;

    readonly List<(GameObject obj, Coroutine routine)> indicators = new();

    public void ActivateIndicator(string name, Sprite icon, float duration)
    {
        if (indicatorPrefab == null || panel == null) return;

        var existing = indicators.Find(i => i.obj.name == name);
        if (existing.obj != null)
        {
            if (existing.routine != null) StopCoroutine(existing.routine);
            indicators.Remove(existing);
            var newRoutine = StartCoroutine(FadeOut(existing.obj, duration));
            indicators.Add((existing.obj, newRoutine));
            return;
        }

        var indicator = Instantiate(indicatorPrefab, panel);
        indicator.name = name;
        
        if (indicator.GetComponentInChildren<Image>() is { } img) img.sprite = icon;
        if (indicator.GetComponentInChildren<TMP_Text>() is { } txt) txt.text = name;

        var routine = StartCoroutine(FadeOut(indicator, duration));
        indicators.Add((indicator, routine));
    }

    IEnumerator FadeOut(GameObject indicator, float duration)
    {
        var img = indicator.GetComponentInChildren<Image>();
        var startAlpha = img != null ? img.color.a : 1f;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            if (img != null) img.color = new(img.color.r, img.color.g, img.color.b, Mathf.Lerp(startAlpha, 0, t / duration));
            yield return null;
        }
        
        indicators.RemoveAll(i => i.obj == indicator);
        Destroy(indicator);
    }
}
