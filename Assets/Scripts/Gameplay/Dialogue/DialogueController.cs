using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.Managers;
using Core.Services;

namespace Gameplay.Dialogue
{
    public sealed class DialogueController : MonoBehaviour, IDialogueController
    {
        [SerializeField] TextMeshProUGUI nameText, dialogueText;
        [Tooltip("The visual dialogue box root. Toggled instead of this GameObject so the controller stays alive to reopen it.")]
        [SerializeField] GameObject dialogueBox;
        [SerializeField] float typeSpeed = 10f;
        [SerializeField] float baseTypeDelay = 0.1f;

        readonly Queue<string> paragraphs = new();
        bool typing, conversationEnded;
        string currentText;
        System.Threading.CancellationTokenSource typeCts;

        float TypeDelay => baseTypeDelay / typeSpeed;

        void Awake() => ServiceLocator.Register<IDialogueController>(this);

        void OnEnable()
        {
            // A re-enabled controller must not resume in a half-finished skip state.
            typing = false;
            conversationEnded = false;
        }

        void OnDisable() => typeCts?.Cancel();

        void OnDestroy()
        {
            typeCts?.Cancel();
            typeCts?.Dispose();
            typeCts = null;
            if (ReferenceEquals(ServiceLocator.Get<IDialogueController>(), this))
                ServiceLocator.Unregister<IDialogueController>();
        }

        public void DisplayNextParagraph(DialogueData dialogue, AudioClip? sound = null)
        {
            if (paragraphs.Count == 0 && conversationEnded)
            {
                EndConversation();
                return;
            }
            if (paragraphs.Count == 0)
            {
                StartConversation(dialogue, sound);
                if (paragraphs.Count == 0)
                {
                    EndConversation();
                    return;
                }
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

        void StartConversation(DialogueData dialogue, AudioClip? sound)
        {
            if (dialogueBox != null) dialogueBox.SetActive(true);
            EventBus.Raise(new DialogueStateChangedEvent(true));
            ServiceLocator.Get<IAudioManager>()?.PlaySound(sound);
            if (nameText != null) nameText.text = dialogue.speakerName;
            foreach (var p in dialogue.paragraphs) paragraphs.Enqueue(p);
        }

        void EndConversation()
        {
            EventBus.Raise(new DialogueStateChangedEvent(false));
            paragraphs.Clear();
            conversationEnded = false;
            typing = false;
            currentText = string.Empty;
            if (dialogueBox != null) dialogueBox.SetActive(false);
        }

        async Awaitable TypeTextAsync(string text)
        {
            typeCts?.Cancel();
            typeCts = new();
            var token = typeCts.Token;
            typing = true;
            dialogueText.text = text;
            dialogueText.maxVisibleCharacters = 0;

            try
            {
                for (int i = 1; i <= text.Length; i++)
                {
                    if (token.IsCancellationRequested) return;
                    dialogueText.maxVisibleCharacters = i;
                    await Awaitable.WaitForSecondsAsync(TypeDelay, token);
                }
            }
            catch (System.OperationCanceledException)
            {
                return;
            }
            // Only the newest task clears the flag; a skipped line restarts typing immediately.
            if (!token.IsCancellationRequested) typing = false;
        }
    }
}
