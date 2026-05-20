using UnityEngine;

namespace Core.Services
{
    [System.Serializable]
    public class DialogueData
    {
        public string speakerName;
        public AudioClip dialogueSound;
        [TextArea(5, 10)] public string[] paragraphs;
    }

    public interface IDialogueService
    {
        void DisplayNextParagraph(DialogueData dialogue, AudioClip sound = null);
    }

    public static class ServiceLocator
    {
        public static IDialogueService Dialogue { get; set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => Dialogue = null;
    }
}
