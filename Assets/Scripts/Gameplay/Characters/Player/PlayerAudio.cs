using UnityEngine;
using Core.Managers;

namespace Gameplay.Characters.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip dashSound;
        
        public void PlayJumpSound()
        {
            SoundManager.instance.PlaySound(jumpSound);
        }

        public void PlayDashSound()
        {
            SoundManager.instance.PlaySound(dashSound);
        }
    }
}
