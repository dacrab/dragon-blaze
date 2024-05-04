using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed =10f;
    [SerializeField] private AudioClip dialogueSound;
    public PlayerMovement playerMovement;
    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private bool isTyping;
    private const float MAX_TYPE_TIME = 0.1f;


    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //If there is nothing in the queue
        if (paragraphs.Count == 0)
        {
            if(!conversationEnded)
            {
                //Start a conversation
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                //End the conversation
                EndConversation();
                return;
            }
        }
        //If there is something in the queue
        if (!isTyping)
        {
            p = paragraphs.Dequeue();
            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));

        }
        //conversation is being typed
        else
        {
            FinishParagraphEarly();
        }

        //update conversationEnded bool
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        //Set animation to idle
        playerMovement.setInteracting(true);

        //Disables the PlayerMovement
        playerMovement.enabled = false; 

        //Plays sound at the start of the dialogue
        SoundManager.instance.PlaySound(dialogueSound);

        //activate gameObject
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }        Time.timeScale = 1f;

        //Update the speakerName
        NPCNameText.text = dialogueText.speakerName;

        //Add dialogue text to queue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }
    private void EndConversation()
    {
        //swich the bool so the other animation can play
        playerMovement.setInteracting(false);

        //Re-Enable PlayerMovement
        playerMovement.enabled = true;

        //Clear the queue
        paragraphs.Clear();
        
        //return bool to false
        conversationEnded = false;

        //Deactivate gameObject
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
        //Stop the coroutine
        StopCoroutine(typeDialogueCoroutine);

        //Finish displaying the text
        NPCDialogueText.maxVisibleCharacters = p.Length;
        NPCDialogueText.text = p;
        
        //Update isTyping bool
        isTyping  = false;
    }
}