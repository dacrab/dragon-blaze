using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    private AudioSource soundSource;
    private AudioSource musicSource;

    //Ref to the sliders
    public Slider musicSlider;
    public Slider soundSlider;

    private void Awake()
{
    soundSource = GetComponent<AudioSource>();
    musicSource = transform.GetChild(0).GetComponent<AudioSource>();

    // Ensure only one instance of SoundManager exists
    if (instance == null)
    {
        instance = this;
    }
    else if (instance != this)
    {
        // Destroy duplicate instances
        Destroy(gameObject);
    }
        //Assing values from PlayerPrefs
        float initialMusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        float initialSoundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
        musicSource.volume = initialMusicVolume;
        soundSource.volume = initialSoundVolume;
        musicSlider.value = initialMusicVolume;
        soundSlider.value = initialSoundVolume;

        //Add listeners for the sliders
        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(musicSlider.value); });
        soundSlider.onValueChanged.AddListener(delegate { ChangeSoundVolume(soundSlider.value); });

    NormalizeSoundVolumes(0.5f); // Normalize all sounds to 50% volume initially
}

    // Play a sound clip once
    public void PlaySound(AudioClip _sound)
    {
        soundSource.PlayOneShot(_sound);
    }

    // Play a sound clip with a specified volume
    public void PlaySoundWithVolume(AudioClip _sound, float volume)
    {
        soundSource.PlayOneShot(_sound, volume);
    }

    // Change the volume of sound effects
    public void ChangeSoundVolume(float _volume)
    {
        soundSource.volume = _volume;
        PlayerPrefs.SetFloat("soundVolume", _volume);    
    }

    // Change the volume of background music
    public void ChangeMusicVolume(float _volume)
    {
        musicSource.volume = _volume;
        PlayerPrefs.SetFloat("musicVolume", _volume);    
    }

    // Change the volume of a specific AudioSource
    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        // Get initial value of volume and change it
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        // Check if we reached the maximum or minimum value
        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        // Assign final value
        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        // Save final value to player prefs
        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }

    public void NormalizeSoundVolumes(float normalizedVolume)
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (var source in audioSources)
        {
            source.volume = normalizedVolume;
        }
        musicSource.volume = normalizedVolume;
        soundSource.volume = normalizedVolume;

        // Update slider values to match the normalized volume
        musicSlider.value = normalizedVolume;
        soundSlider.value = normalizedVolume;
    }
}
