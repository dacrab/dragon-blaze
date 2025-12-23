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
            UpdateVolumeText(PlayerPrefs.GetFloat(volumeName, 0.5f));
            if (SoundManager.Instance == null) return;
            if (volumeName == "musicVolume") SoundManager.Instance.OnMusicVolumeChanged += UpdateVolumeText;
            else if (volumeName == "soundVolume") SoundManager.Instance.OnSoundVolumeChanged += UpdateVolumeText;
        }

        private void OnDisable()
        {
            if (SoundManager.Instance == null) return;
            if (volumeName == "musicVolume") SoundManager.Instance.OnMusicVolumeChanged -= UpdateVolumeText;
            else if (volumeName == "soundVolume") SoundManager.Instance.OnSoundVolumeChanged -= UpdateVolumeText;
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
