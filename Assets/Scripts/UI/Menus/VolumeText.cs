using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI.Menus
{
    public class VolumeText : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private string volumeName; // "musicVolume" or "soundVolume"
        [SerializeField] private string textIntro; // "Sound: " or "Music: "
        #endregion

        #region Private Fields
        private Text txt;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            txt = GetComponent<Text>();
        }

        private void OnEnable()
        {
            // Initial Update
            float currentVol = PlayerPrefs.GetFloat(volumeName, 0.5f);
            UpdateVolumeText(currentVol);

            // Subscribe
            if (SoundManager.instance != null)
            {
                if (volumeName == "musicVolume")
                    SoundManager.instance.OnMusicVolumeChanged += UpdateVolumeText;
                else if (volumeName == "soundVolume")
                    SoundManager.instance.OnSoundVolumeChanged += UpdateVolumeText;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe
            if (SoundManager.instance != null)
            {
                if (volumeName == "musicVolume")
                    SoundManager.instance.OnMusicVolumeChanged -= UpdateVolumeText;
                else if (volumeName == "soundVolume")
                    SoundManager.instance.OnSoundVolumeChanged -= UpdateVolumeText;
            }
        }
        #endregion

        #region Private Methods
        private void UpdateVolumeText(float volumeValue)
        {
            if (txt != null)
                txt.text = $"{textIntro}{(volumeValue * 100):F0}";
        }
        #endregion
    }
}
