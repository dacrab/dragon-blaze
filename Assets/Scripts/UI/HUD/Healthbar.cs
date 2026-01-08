using UnityEngine;
using UnityEngine.UI;
using Core.Events;

namespace UI.HUD;

public sealed class Healthbar : MonoBehaviour
{
    [SerializeField] Image currentHealthBar;

    void OnEnable() => EventBus.OnHealthChanged += UpdateHealthUI;
    void OnDisable() => EventBus.OnHealthChanged -= UpdateHealthUI;

    void UpdateHealthUI(float current, float max)
    {
        if (max > 0 && currentHealthBar != null)
            currentHealthBar.fillAmount = current / max;
    }
}
