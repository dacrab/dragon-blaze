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
            Core.Managers.SoundManager.Instance?.PlaySound(jumpSound);
        }

        public void PlayDashSound()
        {
            Core.Managers.SoundManager.Instance?.PlaySound(dashSound);
        }
    }
}
