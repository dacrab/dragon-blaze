using UnityEngine;

public class Wraith : NPC, ITalkable
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
        //Start Converstation
        dialogueController.DisplayNextParagraph(dialogueText, dialogueSound); // Pass the sound
    }
}
