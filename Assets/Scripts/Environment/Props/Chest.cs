using UnityEngine;

namespace Environment.Props
{
    /// <summary>Plays the open/closed animation of the village chest props.</summary>
    public sealed class Chest : MonoBehaviour
    {
        static readonly int IsOpenedId = Animator.StringToHash("IsOpened");

        [SerializeField] Animator? animator;

        public bool IsOpened
        {
            get => opened;
            set
            {
                opened = value;
                if (animator != null) animator.SetBool(IsOpenedId, opened);
            }
        }

        bool opened;

        public void Open() => IsOpened = true;
        public void Close() => IsOpened = false;
    }
}
