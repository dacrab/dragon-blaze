using UnityEngine;
using UI.Dialogue;

namespace Gameplay.Characters.NPCs;

public sealed class TalkableNPC : NPC
{
    [SerializeField] DialogueText dialogueText;
    [SerializeField] DialogueController dialogueController;
    [SerializeField] AudioClip dialogueSound;

    void Awake() => dialogueController ??= FindFirstObjectByType<DialogueController>();

    protected override void Interact()
    {
        if (dialogueController != null && dialogueText != null)
            dialogueController.DisplayNextParagraph(dialogueText, dialogueSound ?? dialogueText.dialogueSound);
    }
}
