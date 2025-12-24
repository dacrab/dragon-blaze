using UnityEngine;
using Core.Services;
using Core.Utilities;
using Gameplay.Interaction;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs
{
    /// <summary>
    /// NPC that can engage in dialogue with the player.
    /// </summary>
    public class TalkableNPC : NPC, ITalkable
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueText dialogueText;
        [AutoWire(AutoWireAttribute.WireType.Service, required: false)]
        [SerializeField] private DialogueController dialogueController;
        [SerializeField] private AudioClip dialogueSound;

        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
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
