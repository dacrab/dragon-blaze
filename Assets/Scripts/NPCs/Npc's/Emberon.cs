using UnityEngine;

public class Emberon : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioClip dialogueSound; // Added this line

    public override void Interact()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        // Start Conversation
        dialogueController.DisplayNextParagraph(dialogueText, dialogueSound); // Pass the sound
    }
}