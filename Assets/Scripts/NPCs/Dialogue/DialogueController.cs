using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;
    [SerializeField] private AudioClip dialogueSound;
    #endregion

    #region Public Fields
    public PlayerMovement playerMovement;
    #endregion

    #region Private Fields
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
        playerMovement.setInteracting(true);
        playerMovement.enabled = false;

        if (dialogueSound != null)
        {
            SoundManager.instance.PlaySound(dialogueSound);
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        Time.timeScale = 1f;

        NPCNameText.text = dialogueText.speakerName;

        foreach (string paragraph in dialogueText.paragraphs)
        {
            paragraphs.Enqueue(paragraph);
        }
    }

    private void EndConversation()
    {
        playerMovement.setInteracting(false);
        playerMovement.enabled = true;

        paragraphs.Clear();
        conversationEnded = false;

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
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