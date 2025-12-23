using UnityEngine;
using Core.Managers;
using Core.Events;
using TMPro;

namespace UI.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        #region Unity Lifecycle Methods

        private void OnEnable()
        {
            EventBus.OnScoreChanged += UpdateScoreDisplay;
        }

        private void OnDisable()
        {
            EventBus.OnScoreChanged -= UpdateScoreDisplay;
        }

        #endregion

        #region Score Display Methods

        private void UpdateScoreDisplay(int score) => coinText?.SetText($": {score}");

        #endregion
    }
}
