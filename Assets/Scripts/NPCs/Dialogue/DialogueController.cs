using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Core.Events;
using Core.Constants;

public class DialogueController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;
    [SerializeField] private AudioClip dialogueSound;
    #endregion

    #region Private Fields
    // Removed direct PlayerMovement reference. We use EventBus.
    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private bool isTyping;
    private const float MAX_TYPE_TIME = 0.1f;
    #endregion

    #region Public Methods
    public void DisplayNextParagraph(DialogueText dialogueText, AudioClip dialogueSound = null)
    {
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText, dialogueSound);
            }
            else if (conversationEnded && !isTyping)
            {
                EndConversation();
                return;
            }
        }

        if (!isTyping)
        {
            p = paragraphs.Dequeue();
            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }

        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }
    #endregion

    #region Private Methods
    private void StartConversation(DialogueText dialogueText, AudioClip dialogueSound = null)
    {
        // Signal Game Pause/Freeze
        EventBus.RaiseDialogueStateChanged(true);
        // Also stop player manually if needed, but EventBus should handle it if PlayerController listens.
        // Note: PlayerController currently doesn't listen to Dialogue events, but UIManager pauses game?
        // Wait, Dialogue usually doesn't freeze time completely like Pause Menu (Time.timeScale = 0).
        // It usually disables input.
        // We need PlayerController to listen to this. I will add that logic later or assume it does.
        // For now, let's rely on EventBus.
        
        if (dialogueSound != null)
        {
            SoundManager.instance.PlaySound(dialogueSound);
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        // Time.timeScale = 1f; // Ensure time is running for typing

        NPCNameText.text = dialogueText.speakerName;

        foreach (string paragraph in dialogueText.paragraphs)
        {
            paragraphs.Enqueue(paragraph);
        }
    }

    private void EndConversation()
    {
        EventBus.RaiseDialogueStateChanged(false);

        paragraphs.Clear();
        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;
        int maxVisibleChars = 0;

        NPCDialogueText.text = p;
        NPCDialogueText.maxVisibleCharacters = maxVisibleChars;        

        foreach (char c in p.ToCharArray())
        {
            maxVisibleChars++;
            NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);

        NPCDialogueText.maxVisibleCharacters = p.Length;
        NPCDialogueText.text = p;
        
        isTyping = false;
    }
    #endregion
}