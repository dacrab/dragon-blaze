using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public PlayerMovement player; // Reference to the PlayerMovement script
    public TextMeshProUGUI coinText;
    private void Start()
    {
        coinText = GetComponent<TextMeshProUGUI>(); // Get the Text component attached to this GameObject
    }

private void Update()
{
    int playerScore = player.GetScore(); // Get the player's score

    // Display the score only if it's greater than 0
    if (playerScore > 0)
    {
        coinText.text = $": {playerScore}"; // Update the text dynamically
    }
    else
    {
        coinText.text = " "; // Hide the score when it's 0
    }
}
}
