using UnityEngine;

namespace Gameplay.Dialogue
{
    /// <summary>Contract implemented by the DialogueController.</summary>
    public interface IDialogueController
    {
        void DisplayNextParagraph(DialogueData dialogue, AudioClip sound = null);
    }
}
