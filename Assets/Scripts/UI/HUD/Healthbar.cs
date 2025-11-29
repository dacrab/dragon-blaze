using UnityEngine;
using UnityEngine.UI;
using Core.Events;

namespace UI.HUD
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private Image totalHealthBar;
        [SerializeField] private Image currentHealthBar;

        private void Awake()
        {
            if (totalHealthBar == null || currentHealthBar == null)
            {
                var images = GetComponentsInChildren<Image>();
                if (images.Length >= 2)
                {
                    totalHealthBar ??= images[0];
                    currentHealthBar ??= images[1];
                }
            }
        }

        private void OnEnable() => EventBus.OnHealthChanged += UpdateHealthUI;
        private void OnDisable() => EventBus.OnHealthChanged -= UpdateHealthUI;

        private void UpdateHealthUI(float current, float max)
        {
            if (max > 0 && currentHealthBar != null)
                currentHealthBar.fillAmount = current / max;
        }
    }
}
