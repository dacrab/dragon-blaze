using UnityEngine;

public class Lumina : NPC, ITalkable
{
    #region Serialized Fields
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioClip dialogueSound;
    #endregion

    #region Public Methods
    public override void Interact()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText, dialogueSound);
    }
    #endregion
}
