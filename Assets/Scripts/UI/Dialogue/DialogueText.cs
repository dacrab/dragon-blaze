using UnityEngine;

namespace UI.Dialogue;

[CreateAssetMenu(fileName = "DialogueText", menuName = "DragonBlaze/Dialogue/Dialogue Text")]
public sealed class DialogueText : ScriptableObject
{
    public string speakerName;
    public AudioClip dialogueSound;
    
    [TextArea(5, 10)]
    public string[] paragraphs;
}
