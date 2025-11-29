using UnityEngine;
using Core.Events;
using TMPro;

namespace UI.HUD
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private string format = ": {0}";

        private void Awake()
        {
            if (coinText == null)
                coinText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable() => EventBus.OnScoreChanged += UpdateScoreDisplay;
        private void OnDisable() => EventBus.OnScoreChanged -= UpdateScoreDisplay;

        private void UpdateScoreDisplay(int score)
        {
            if (coinText != null)
                coinText.text = string.Format(format, score);
        }
    }
}
