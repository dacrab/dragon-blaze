using UnityEngine;
using UnityEngine.UI;
using Core.Constants;
using Core.Managers;
using Core.Services;

namespace UI.HUD
{
    /// <summary>
    /// Slider that drives one of the AudioManager volume channels. Wires itself to its own
    /// Slider component, so scene wiring only means adding this script to the slider object.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public sealed class VolumeSlider : MonoBehaviour
    {
        [SerializeField] bool musicChannel;

        Slider slider;

        void Awake()
        {
            slider = GetComponent<Slider>();
            slider.SetValueWithoutNotify(musicChannel
                ? Mathf.Clamp01(ServiceLocator.Get<IAudioManager>()?.MusicVolume ?? GameConfig.Default.defaultMusicVolume)
                : Mathf.Clamp01(ServiceLocator.Get<IAudioManager>()?.SoundVolume ?? GameConfig.Default.defaultSoundVolume));
            slider.onValueChanged.AddListener(OnChanged);
        }

        void OnDestroy() => slider.onValueChanged.RemoveListener(OnChanged);

        void OnChanged(float value)
        {
            var audio = ServiceLocator.Get<IAudioManager>();
            if (audio == null) return;
            if (musicChannel) audio.SetMusicVolume(value);
            else audio.SetSoundVolume(value);
        }
    }
}
