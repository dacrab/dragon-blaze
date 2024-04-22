using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/New Dialogue Container")]
public class DialogueText : ScriptableObject
{

    [TextArea(5,10)]
    public string speakerName;
    public string[] paragraphs;
}
