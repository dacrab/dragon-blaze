using UnityEngine;
using TMPro;
using Core.Managers;

namespace UI.Menus
{

public enum VolumeType { Music, Sound }

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class VolumeText : MonoBehaviour
{
    [SerializeField] VolumeType volumeType;
    [SerializeField] string prefix = "";

    TextMeshProUGUI text;

    void Awake() => text = GetComponent<TextMeshProUGUI>();

    void OnEnable()
    {
        UpdateText(volumeType == VolumeType.Music ? SoundManager.Instance?.MusicVolume ?? 0.5f : SoundManager.Instance?.SoundVolume ?? 0.5f);
        if (SoundManager.Instance == null) return;
        if (volumeType == VolumeType.Music) SoundManager.Instance.OnMusicVolumeChanged += UpdateText;
        else SoundManager.Instance.OnSoundVolumeChanged += UpdateText;
    }

    void OnDisable()
    {
        if (SoundManager.Instance == null) return;
        if (volumeType == VolumeType.Music) SoundManager.Instance.OnMusicVolumeChanged -= UpdateText;
        else SoundManager.Instance.OnSoundVolumeChanged -= UpdateText;
    }

    void UpdateText(float value) => text.text = $"{prefix}{value * 100:F0}";
}
}