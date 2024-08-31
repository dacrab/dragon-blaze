using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Audio Sources")]
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource musicSource;

    [Header("UI References")]
    public Slider musicSlider;
    public Slider soundSlider;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        InitializeSingleton();
        InitializeAudioSources();
        InitializeSliders();
    }

    private void Start()
    {
        // Removed PlayLevelMusic() call as levelMusic has been removed
    }
    #endregion

    #region Initialization Methods
    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
            // Remove DontDestroyOnLoad to allow each scene to have its own SoundManager
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (soundSource == null) soundSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        float savedSoundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
        musicSource.volume = savedMusicVolume;
        soundSource.volume = savedSoundVolume;
    }

    private void InitializeSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        }

        if (soundSlider != null)
        {
            soundSlider.value = PlayerPrefs.GetFloat("soundVolume", 0.5f);
            soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
        }
    }
    #endregion

    #region Sound Playback Methods
    public void PlaySound(AudioClip _sound)
    {
        soundSource.PlayOneShot(_sound);
    }

    public void PlaySoundWithVolume(AudioClip _sound, float volume)
    {
        soundSource.PlayOneShot(_sound, volume);
    }
    #endregion

    #region Volume Control Methods
    public void ChangeSoundVolume(float _volume)
    {
        soundSource.volume = _volume;
        PlayerPrefs.SetFloat("soundVolume", _volume);
        PlayerPrefs.Save();

        if (soundSlider != null)
        {
            soundSlider.value = _volume;
        }
    }

    public void ChangeMusicVolume(float _volume)
    {
        musicSource.volume = _volume;
        PlayerPrefs.SetFloat("musicVolume", _volume);
        PlayerPrefs.Save();

        if (musicSlider != null)
        {
            musicSlider.value = _volume;
        }
    }

    private void ChangeSourceVolume(float baseVolume, string volumeName, float change, AudioSource source)
    {
        float currentVolume = PlayerPrefs.GetFloat(volumeName, 1);
        currentVolume += change;

        if (currentVolume > 1)
            currentVolume = 0;
        else if (currentVolume < 0)
            currentVolume = 1;

        float finalVolume = currentVolume * baseVolume;
        source.volume = finalVolume;

        PlayerPrefs.SetFloat(volumeName, currentVolume);
    }
    #endregion
}