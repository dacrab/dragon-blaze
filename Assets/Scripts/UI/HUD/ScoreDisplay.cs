using UnityEngine;
using Core.Events;
using TMPro;

namespace UI.HUD;

public sealed class ScoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;

    void OnEnable() => EventBus.OnScoreChanged += UpdateScore;
    void OnDisable() => EventBus.OnScoreChanged -= UpdateScore;

    void UpdateScore(int score) => coinText?.SetText($": {score}");
}
