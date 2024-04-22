using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private int MAX_TYPE_TIME = 10;
    [SerializeField] private TextMeshProUGUI NPCNametext;
    [SerializeField] private TextMeshProUGUI NPCDialoguetext;
    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private string p;
    private Coroutine typeDialogueCoroutine;
    private bool isTyping;
    private int typeSpeed;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        //If there is nothing in the queue
        if (paragraphs.Count == 0)
        {
            if (conversationEnded)
            {
                //Start a conversation
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                //end a conversation
                EndConversation();
                return;
            }
        }
        //if there is something in the queue
        if (!isTyping)
        {
          p = paragraphs.Dequeue();  
          typeDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }
        
        //Update Conversation bool
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }
    private void StartConversation(DialogueText dialogueText)
    {
        //Activate GameObject
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        //Update the speaker name
        NPCNametext.text = dialogueText.speakerName;

        //Add dialogue text to the queue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }

    private void EndConversation()
    {
        //Clear the queue
        paragraphs.Clear();

        //Return bool to false
        conversationEnded = false;

        //Deactivate gameobject
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        int maxVisibleChars = 0;

        NPCDialoguetext.text = p;
        NPCDialoguetext.maxVisibleCharacters = maxVisibleChars;        

        foreach (char c in p.ToCharArray())
        {

            maxVisibleChars++;
            NPCDialoguetext.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        //Stop coroutine
        StopCoroutine(typeDialogueCoroutine);

        //Finish Displaying text
        NPCDialoguetext.text = p;

        //Update isTyping bool
        isTyping = false;
    }

}
