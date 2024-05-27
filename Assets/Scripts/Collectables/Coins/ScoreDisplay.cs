using UnityEngine;
using TMPro; // Add this for TextMeshProUGUI
using System.Collections; // Add this for IEnumerator

public class ScoreDisplay : MonoBehaviour // Make sure to inherit from MonoBehaviour
{
    public TextMeshProUGUI coinText;

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScoreDisplay;  // Subscribe to the correct event
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScoreDisplay;  // Unsubscribe when disabled
    }

    private void UpdateScoreDisplay(int score)
    {
        if (coinText != null)
        {
            coinText.text = $": {score}";
            Debug.Log($"Score updated to: {score}");
        }
        else
        {
            Debug.LogError("coinText is null in ScoreDisplay");
        }
    }
}
