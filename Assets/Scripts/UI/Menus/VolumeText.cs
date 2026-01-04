using UnityEngine;
using UnityEngine.UI;
using Core.Constants;
using Core.Managers;

namespace UI.Menus
{
    public class VolumeText : MonoBehaviour
    {
        [SerializeField] private string volumeName;
        [SerializeField] private string textIntro;
        private Text txt;

        private void Awake()
        {
            txt = GetComponent<Text>();
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

        private void UpdateVolumeText(float value)
        {
            if (txt != null) txt.text = $"{textIntro}{(value * 100):F0}";
        }
    }
}
