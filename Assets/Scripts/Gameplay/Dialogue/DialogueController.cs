using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Core.Events;
using Core.Managers;
using Core.Services;

namespace Gameplay.Dialogue
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "DragonBlaze/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public string speakerName;
        public AudioClip dialogueSound;
        [TextArea(5, 10)] public string[] paragraphs;
    }

    public sealed class DialogueController : MonoBehaviour, IDialogueController
    {
        [SerializeField] TextMeshProUGUI nameText, dialogueText;
        [SerializeField] float typeSpeed = 10f;
        [SerializeField] float baseTypeDelay = 0.1f;

        readonly Queue<string> paragraphs = new();
        bool typing, conversationEnded;
        string currentText;
        System.Threading.CancellationTokenSource typeCts;

        float TypeDelay => baseTypeDelay / typeSpeed;

        void OnEnable() => ServiceLocator.Register<IDialogueController>(this);

        void OnDisable() => typeCts?.Cancel();

        void OnDestroy()
        {
            if (ReferenceEquals(ServiceLocator.Get<IDialogueController>(), this))
                ServiceLocator.Unregister<IDialogueController>();
        }

        public void DisplayNextParagraph(DialogueData dialogue, AudioClip sound = null)
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

        void StartConversation(DialogueData dialogue, AudioClip sound)
        {
            EventBus.Raise(new DialogueStateChangedEvent(true));
            ServiceLocator.Get<IAudioManager>()?.PlaySound(sound);
            gameObject.SetActive(true);
            nameText.text = dialogue.speakerName;
            foreach (var p in dialogue.paragraphs) paragraphs.Enqueue(p);
        }

        void EndConversation()
        {
            EventBus.Raise(new DialogueStateChangedEvent(false));
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
    }
}
