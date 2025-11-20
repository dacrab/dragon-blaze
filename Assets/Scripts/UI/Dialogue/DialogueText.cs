using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
public class DialogueText : ScriptableObject
{
    #region Public Fields
    public string speakerName;
    public AudioClip dialogueSound;
    #endregion

    #region Serialized Fields
    [TextArea(5, 10)]
    public string[] paragraphs;
    #endregion
}
