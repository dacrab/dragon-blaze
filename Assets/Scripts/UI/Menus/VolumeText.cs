using UnityEngine;
using UnityEngine.UI;
using Core.Managers;

namespace UI.Menus
{
    public enum VolumeType { Music, Sound }

    [RequireComponent(typeof(Text))]
    public class VolumeText : MonoBehaviour
    {
        [SerializeField] private VolumeType volumeType;
        [SerializeField] private string displayFormat = "{0}: {1:F0}";

        private Text txt;
        private string prefsKey;

        private void Awake()
        {
            txt = GetComponent<Text>();
            prefsKey = volumeType == VolumeType.Music ? "musicVolume" : "soundVolume";
        }

        private void OnEnable()
        {
            UpdateVolumeText(PlayerPrefs.GetFloat(prefsKey, 0.5f));

            if (SoundManager.Instance != null)
            {
                if (volumeType == VolumeType.Music)
                    SoundManager.Instance.OnMusicVolumeChanged += UpdateVolumeText;
                else
                    SoundManager.Instance.OnSoundVolumeChanged += UpdateVolumeText;
            }
        }

        private void OnDisable()
        {
            if (SoundManager.Instance != null)
            {
                if (volumeType == VolumeType.Music)
                    SoundManager.Instance.OnMusicVolumeChanged -= UpdateVolumeText;
                else
                    SoundManager.Instance.OnSoundVolumeChanged -= UpdateVolumeText;
            }
        }

        private void UpdateVolumeText(float volumeValue)
        {
            if (txt != null)
                txt.text = string.Format(displayFormat, volumeType, volumeValue * 100);
        }
    }
}
