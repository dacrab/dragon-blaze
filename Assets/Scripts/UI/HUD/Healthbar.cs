using UnityEngine;
using UnityEngine.UI;
using Core.Events;

namespace UI.HUD
{
    public class Healthbar : MonoBehaviour
    {
        #region Serialized Fields
        // Removed direct PlayerHealth reference
        [SerializeField] private Image totalhealthBar;
        [SerializeField] private Image currenthealthBar;
        #endregion

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            EventBus.OnHealthChanged += UpdateHealthUI;
        }

        private void OnDisable()
        {
            EventBus.OnHealthChanged -= UpdateHealthUI;
        }
        #endregion

        #region Public Methods
        public void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (maxHealth > 0 && currenthealthBar != null)
            {
                currenthealthBar.fillAmount = currentHealth / maxHealth;
            }
        }
        #endregion
    }
}
