using UnityEngine.UI;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public PlayerMovement player; // Reference to the PlayerMovement script
    private Text text; // Reference to the Text component

    private void Start()
    {
        text = GetComponent<Text>(); // Get the Text component attached to this GameObject
    }

    private void Update()
    {
        text.text = "Total Coins: " + player.GetScore(); // Update the text to display the player's score
    }
}
