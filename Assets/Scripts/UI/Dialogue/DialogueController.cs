using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.Managers;
using Core.Services;

namespace UI.Dialogue
{
    public sealed class DialogueController : MonoBehaviour, IDialogueService
    {
        [SerializeField] TextMeshProUGUI nameText, dialogueText;
        [SerializeField] float typeSpeed = 10f;
        [SerializeField] float baseTypeDelay = 0.1f;

        readonly Queue<string> paragraphs = new();
        bool typing, conversationEnded;
        string currentText;
        System.Threading.CancellationTokenSource typeCts;

        float TypeDelay => baseTypeDelay / typeSpeed;

        void Awake() => ServiceLocator.Dialogue = this;
        void OnDestroy() { if (ServiceLocator.Dialogue == (IDialogueService)this) ServiceLocator.Dialogue = null; }

        public void DisplayNextParagraph(DialogueData dialogue, AudioClip sound = null)
        {
            if (paragraphs.Count == 0)
            {
                if (!conversationEnded) StartConversation(dialogue, sound);
                else { EndConversation(); return; }
            }

            if (typing)
            {
                typeCts?.Cancel();
                dialogueText.maxVisibleCharacters = currentText.Length;
                typing = false;
                return;
            }

            currentText = paragraphs.Dequeue();
            _ = TypeTextAsync(currentText);
            if (paragraphs.Count == 0) conversationEnded = true;
        }

        void StartConversation(DialogueData dialogue, AudioClip sound)
        {
            EventBus.RaiseDialogueStateChanged(true);
            GameManager.Instance?.PlaySound(sound);
            gameObject.SetActive(true);
            nameText.text = dialogue.speakerName;
            foreach (var p in dialogue.paragraphs) paragraphs.Enqueue(p);
        }

        void EndConversation()
        {
            EventBus.RaiseDialogueStateChanged(false);
            paragraphs.Clear();
            conversationEnded = false;
            gameObject.SetActive(false);
        }

        async Awaitable TypeTextAsync(string text)
        {
            typeCts?.Cancel();
            typeCts = new();
            typing = true;
            dialogueText.text = text;
            dialogueText.maxVisibleCharacters = 0;

            for (int i = 1; i <= text.Length; i++)
            {
                if (typeCts.Token.IsCancellationRequested) break;
                dialogueText.maxVisibleCharacters = i;
                await Awaitable.WaitForSecondsAsync(TypeDelay);
            }
            typing = false;
        }

        void OnDisable() => typeCts?.Cancel();
    }
}
