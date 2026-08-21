using UnityEngine;
using UnityEngine.UI;
using Core.Events;

namespace UI.HUD
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] Image currentHealthBar;

        void OnEnable() => EventBus.Subscribe<HealthChangedEvent>(UpdateHealthUI);
        void OnDisable() => EventBus.Unsubscribe<HealthChangedEvent>(UpdateHealthUI);

        void UpdateHealthUI(HealthChangedEvent e)
        {
            if (e.Max > 0 && currentHealthBar != null)
                currentHealthBar.fillAmount = e.Current / e.Max;
        }
    }
}
