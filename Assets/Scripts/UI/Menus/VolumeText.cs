using UnityEngine;
using UnityEngine.UI;
using Core.Constants;
using Core.Managers;
using Core.Utilities;

namespace UI.Menus
{
    public class VolumeText : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private string volumeName; // "musicVolume" or "soundVolume"
        [SerializeField] private string textIntro; // "Sound: " or "Music: "
        #endregion

        #region Private Fields
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private Text txt;
        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            Core.Utilities.AutoWireHelper.WireAllFields(this);
        }

        private void OnEnable()
        {
            UpdateVolumeText(PlayerPrefs.GetFloat(volumeName, 0.5f));
            if (SoundManager.Instance == null) return;
            if (volumeName == GameConstants.Save.MusicVolume) SoundManager.Instance.OnMusicVolumeChanged += UpdateVolumeText;
            else if (volumeName == GameConstants.Save.SoundVolume) SoundManager.Instance.OnSoundVolumeChanged += UpdateVolumeText;
        }

        private void OnDisable()
        {
            if (SoundManager.Instance == null) return;
            if (volumeName == GameConstants.Save.MusicVolume) SoundManager.Instance.OnMusicVolumeChanged -= UpdateVolumeText;
            else if (volumeName == GameConstants.Save.SoundVolume) SoundManager.Instance.OnSoundVolumeChanged -= UpdateVolumeText;
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
