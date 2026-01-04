using UnityEngine;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs
{
    public class TalkableNPC : NPC
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueText dialogueText;
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioClip dialogueSound;

        private void Awake()
        {
            if (dialogueController == null)
                dialogueController = FindFirstObjectByType<DialogueController>();
        }

        protected override void Interact()
        {
            if (dialogueController != null && dialogueText != null)
                dialogueController.DisplayNextParagraph(dialogueText, dialogueSound ?? dialogueText.dialogueSound);
        }
    }
}
