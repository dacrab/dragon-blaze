using UnityEngine;
using TMPro; // Add this for TextMeshProUGUI
using System.Collections; // Add this for IEnumerator

public class ScoreDisplay : MonoBehaviour // Make sure to inherit from MonoBehaviour
{
    public TextMeshProUGUI coinText;

    private void OnEnable()
    {
        PlayerMovement.OnScoreChanged += UpdateScoreDisplay;  // Make sure this is the correct event
    }

    private void OnDisable()
    {
        PlayerMovement.OnScoreChanged -= UpdateScoreDisplay;
    }

    private void UpdateScoreDisplay(int score)
    {
        Debug.Log($"Received score update: {score}");
        if (coinText != null)
        {
            coinText.text = $": {score}";
        }
        else
        {
            Debug.LogError("coinText is null");
        }
    }
}
