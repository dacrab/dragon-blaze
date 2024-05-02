using UnityEngine;

public class Emberon : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    public override void Interact()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        //Start Converstation
        dialogueController.DisplayNextParagraph(dialogueText);
    }
}