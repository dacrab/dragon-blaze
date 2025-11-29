using UnityEngine;
using Core.Managers;
using Core.Events;
using TMPro;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UI.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float typeSpeed = 10f;

        private readonly Queue<string> paragraphs = new();
        private bool conversationEnded;
        private string currentParagraph;
        private bool isTyping;
        private CancellationTokenSource typingCts;

        private const float MAX_TYPE_TIME = 0.1f;

        private void Awake()
        {
            if (nameText == null || dialogueText == null)
            {
                var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
                if (texts.Length > 0 && nameText == null) nameText = texts[0];
                if (texts.Length > 1 && dialogueText == null) dialogueText = texts[1];
            }
        }

        private void OnDestroy() => typingCts?.Cancel();

        public void DisplayNextParagraph(DialogueText dialogue, AudioClip sound = null)
        {
            if (paragraphs.Count == 0)
            {
                if (!conversationEnded)
                {
                    StartConversation(dialogue, sound);
                }
                else if (!isTyping)
                {
                    EndConversation();
                    return;
                }
            }

            if (!isTyping && paragraphs.Count > 0)
            {
                currentParagraph = paragraphs.Dequeue();
                typingCts?.Cancel();
                typingCts = new CancellationTokenSource();
                TypeTextAsync(currentParagraph, typingCts.Token).Forget();
            }
            else if (isTyping)
            {
                FinishParagraphEarly();
            }

            if (paragraphs.Count == 0)
                conversationEnded = true;
        }

        private void StartConversation(DialogueText dialogue, AudioClip sound)
        {
            EventBus.RaiseDialogueStateChanged(true);
            SoundManager.Instance?.PlaySound(sound);

            gameObject.SetActive(true);
            nameText.text = dialogue.speakerName;

            foreach (var paragraph in dialogue.paragraphs)
                paragraphs.Enqueue(paragraph);
        }

        private void EndConversation()
        {
            EventBus.RaiseDialogueStateChanged(false);
            paragraphs.Clear();
            conversationEnded = false;
            gameObject.SetActive(false);
        }

        private async UniTaskVoid TypeTextAsync(string text, CancellationToken token)
        {
            isTyping = true;
            dialogueText.text = text;
            dialogueText.maxVisibleCharacters = 0;

            int delay = (int)(MAX_TYPE_TIME / typeSpeed * 1000);
            
            for (int i = 1; i <= text.Length; i++)
            {
                if (token.IsCancellationRequested) break;
                dialogueText.maxVisibleCharacters = i;
                await UniTask.Delay(delay, cancellationToken: token);
            }

            isTyping = false;
        }

        private void FinishParagraphEarly()
        {
            typingCts?.Cancel();
            dialogueText.maxVisibleCharacters = currentParagraph.Length;
            dialogueText.text = currentParagraph;
            isTyping = false;
        }
    }
}
