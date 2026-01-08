using UnityEngine;
using Core.Managers;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Core.Events;

namespace UI.Dialogue
{

public sealed class DialogueController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI nameText, dialogueText;
    
    [Header("Typing Settings")]
    [SerializeField] float typeSpeed = 10f;
    [SerializeField] float baseTypeDelay = 0.1f;

    readonly Queue<string> paragraphs = new();
    Coroutine typeRoutine;
    string currentText;
    bool conversationEnded;

    float TypeDelay => baseTypeDelay / typeSpeed;

    public void DisplayNextParagraph(DialogueText dialogue, AudioClip sound = null)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded) StartConversation(dialogue, sound);
            else { EndConversation(); return; }
        }

        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            dialogueText.maxVisibleCharacters = currentText.Length;
            typeRoutine = null;
            return;
        }

        currentText = paragraphs.Dequeue();
        typeRoutine = StartCoroutine(TypeText(currentText));
        if (paragraphs.Count == 0) conversationEnded = true;
    }

    void StartConversation(DialogueText dialogue, AudioClip sound)
    {
        EventBus.DialogueStateChanged(true);
        SoundManager.Instance?.PlaySound(sound);
        gameObject.SetActive(true);
        nameText.text = dialogue.speakerName;
        foreach (var p in dialogue.paragraphs) paragraphs.Enqueue(p);
    }

    void EndConversation()
    {
        EventBus.DialogueStateChanged(false);
        paragraphs.Clear();
        conversationEnded = false;
        gameObject.SetActive(false);
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;
        for (int i = 1; i <= text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(TypeDelay);
        }
        typeRoutine = null;
    }
}
}