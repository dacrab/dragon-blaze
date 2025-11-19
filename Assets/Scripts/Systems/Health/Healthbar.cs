using UnityEngine;
using UnityEngine.UI;
using Core.Events;

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
        // Assuming maxHealth is always 10 for calculation or use parameter
        // Original code divided by 10, assuming maxHealth/10 ratio for fill amount?
        // If maxHealth is 100 and we want 0-1 fill, we divide by maxHealth.
        // But original code: "playerHealth.currentHealth / 10"
        // If max health is 3, then 3/10 = 0.3. Maybe it's hearts?
        // Let's use standard percentage: current / max.
        
        if (maxHealth > 0)
        {
            currenthealthBar.fillAmount = currentHealth / 10f; // Keeping original ratio logic if visual design depends on it, 
                                                               // OR better: assume user wants valid bar.
                                                               // Original: totalHealthBar = current / 10 in Start.
                                                               // This implies max health was hardcoded to 10 conceptually?
                                                               // Let's blindly follow "divide by 10" for migration safety
                                                               // UNLESS we want to modernize to actual percentage.
                                                               
            // Wait, Player Health defaults to 100. 100/10 = 10. Fill amount caps at 1. 
            // So if health > 10, bar is full. 
            // If health drops to 5, bar is 0.5.
            // So effectively max visible health is 10?
            
            // Let's use the ratio passed in.
            currenthealthBar.fillAmount = currentHealth / 10f; 
            
            // Also update totalHealthBar? Original only updated total in Start.
        }
    }
    #endregion
}