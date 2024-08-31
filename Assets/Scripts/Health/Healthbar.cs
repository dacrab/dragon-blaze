using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeTotalHealthBar();
    }

    private void Update()
    {
        UpdateCurrentHealthBar();
    }
    #endregion

    #region Public Methods
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        currenthealthBar.fillAmount = currentHealth / maxHealth;
    }
    #endregion

    #region Private Methods
    private void InitializeTotalHealthBar()
    {
        totalhealthBar.fillAmount = playerHealth.currentHealth / 10;
    }

    private void UpdateCurrentHealthBar()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / 10;
    }
    #endregion
}