using UnityEngine;
using Core.Managers;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Core.Events;

namespace UI.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI NPCNameText;
        [SerializeField] private TextMeshProUGUI NPCDialogueText;
        [SerializeField] private float typeSpeed = 10f;

        private Queue<string> paragraphs = new();
        private bool conversationEnded;
        private string currentParagraph;
        private Coroutine typeCoroutine;
        private bool isTyping;

        public void DisplayNextParagraph(DialogueText dialogueText, AudioClip dialogueSound = null)
        {
            if (paragraphs.Count == 0)
            {
                if (!conversationEnded) StartConversation(dialogueText, dialogueSound);
                else if (!isTyping) { EndConversation(); return; }
            }

            if (!isTyping)
            {
                currentParagraph = paragraphs.Dequeue();
                typeCoroutine = StartCoroutine(TypeDialogueText(currentParagraph));
            }
            else FinishParagraphEarly();

            if (paragraphs.Count == 0) conversationEnded = true;
        }

        private void StartConversation(DialogueText dialogueText, AudioClip dialogueSound = null)
        {
            EventBus.RaiseDialogueStateChanged(true);
            if (dialogueSound != null) SoundManager.Instance?.PlaySound(dialogueSound);
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            NPCNameText.text = dialogueText.speakerName;
            foreach (string paragraph in dialogueText.paragraphs) paragraphs.Enqueue(paragraph);
        }

        private void EndConversation()
        {
            EventBus.RaiseDialogueStateChanged(false);
            paragraphs.Clear();
            conversationEnded = false;
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        private IEnumerator TypeDialogueText(string text)
        {
            isTyping = true;
            NPCDialogueText.text = text;
            NPCDialogueText.maxVisibleCharacters = 0;

            for (int i = 1; i <= text.Length; i++)
            {
                NPCDialogueText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(0.1f / typeSpeed);
            }
            isTyping = false;
        }

        private void FinishParagraphEarly()
        {
            StopCoroutine(typeCoroutine);
            NPCDialogueText.maxVisibleCharacters = currentParagraph.Length;
            isTyping = false;
        }
    }
}
