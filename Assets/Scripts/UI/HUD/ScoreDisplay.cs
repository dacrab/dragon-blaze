using UnityEngine;
using Core.Events;
using TMPro;

namespace UI.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        private void OnEnable() => EventBus.OnScoreChanged += UpdateScoreDisplay;
        private void OnDisable() => EventBus.OnScoreChanged -= UpdateScoreDisplay;

        private void UpdateScoreDisplay(int score) => coinText?.SetText($": {score}");
    }
}
