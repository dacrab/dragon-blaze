using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip dashSound;
        
        public void PlayJumpSound()
        {
            if (jumpSound != null)
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        }

        public void PlayDashSound()
        {
            if (dashSound != null)
                AudioSource.PlayClipAtPoint(dashSound, transform.position);
        }
    }
}
