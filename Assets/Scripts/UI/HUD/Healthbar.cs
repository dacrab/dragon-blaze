using UnityEngine;
using UnityEngine.UI;
using Core.Events;

namespace UI.HUD
{
    public class Healthbar : MonoBehaviour
    {
        [SerializeField] private Image currenthealthBar;

        private void OnEnable() => EventBus.OnHealthChanged += UpdateHealthUI;
        private void OnDisable() => EventBus.OnHealthChanged -= UpdateHealthUI;

        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (maxHealth > 0 && currenthealthBar != null)
                currenthealthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}
