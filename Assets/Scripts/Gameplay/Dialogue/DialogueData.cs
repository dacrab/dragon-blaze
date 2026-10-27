using UnityEngine;

namespace Gameplay.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "DragonBlaze/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public string speakerName;
        public AudioClip dialogueSound;
        [TextArea(5, 10)] public string[] paragraphs;
    }
}
