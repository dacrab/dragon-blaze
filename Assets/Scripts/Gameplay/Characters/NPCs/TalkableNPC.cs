using UnityEngine;
using Gameplay.Interaction;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs
{
    public class TalkableNPC : NPC, ITalkable
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

        public override void Interact() => Talk();

        public void Talk() => Talk(dialogueText);

        public void Talk(DialogueText text)
        {
            if (dialogueController != null && text != null)
            {
                dialogueController.DisplayNextParagraph(text, dialogueSound ?? text.dialogueSound);
            }
        }
    }
}
