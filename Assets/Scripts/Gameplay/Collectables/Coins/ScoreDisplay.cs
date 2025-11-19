using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    #region Unity Lifecycle Methods

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScoreDisplay;
    }

    #endregion

    #region Score Display Methods

    private void UpdateScoreDisplay(int score)
    {
        if (coinText != null)
        {
            coinText.text = $": {score}";
        }
    }

    #endregion
}
