using UnityEngine;
using Gameplay.Characters.NPCs;
using Gameplay.Interaction;
using UI.Dialogue;

public class Lumina : NPC, ITalkable
{
    #region Serialized Fields
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private AudioClip dialogueSound;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (dialogueController == null)
        {
            dialogueController = FindFirstObjectByType<DialogueController>();
        }
    }
    #endregion

    #region Public Methods
    public override void Interact()
    {
        Talk();
    }

    public void Talk()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText, dialogueSound);
    }
    #endregion
}
