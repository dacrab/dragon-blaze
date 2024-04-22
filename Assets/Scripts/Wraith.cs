using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    public override void Interact()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        //Start the dialogue
        dialogueController.DisplayNextParagraph(dialogueText);
    }
}
