using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Core.Events;
using Core.Constants;

namespace Core.Managers
{
    [System.Serializable]
    public sealed class SaveData
    {
        public int totalCoins;
        public int currentLevel;
    }

    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public event Action<float> OnSoundVolumeChanged, OnMusicVolumeChanged;

        [SerializeField] GameConfig gameConfig;
        [SerializeField] AudioSource soundSource, musicSource;
        string SavePath => Application.persistentDataPath + "/" + (gameConfig?.saveFileName ?? "savefile.json");

        public int TotalCoins { get; private set; }
        public float SoundVolume => soundSource.volume;
        public float MusicVolume => musicSource.volume;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else Destroy(gameObject);
        }

        void Initialize()
        {
            if (soundSource == null || musicSource == null)
            {
                Debug.LogError("GameManager requires AudioSource components assigned in inspector");
            }
            else
            {
                musicSource.volume = PlayerPrefs.GetFloat(gameConfig.musicVolumeKey, gameConfig.defaultMusicVolume);
                soundSource.volume = PlayerPrefs.GetFloat(gameConfig.soundVolumeKey, gameConfig.defaultSoundVolume);
            }
            
            LoadGame();
            EventBus.OnLevelCompleted += SaveGame;
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                EventBus.OnLevelCompleted -= SaveGame;
                Instance = null;
            }
        }

        public void AddCoins(int value)
        {
            if (value <= 0) return;
            TotalCoins += value;
            EventBus.OnScoreChanged?.Invoke(TotalCoins);
        }

        public void ResetCoins()
        {
            TotalCoins = 0;
            EventBus.OnScoreChanged?.Invoke(TotalCoins);
        }

        public void SaveGame(bool isNewGame = false)
        {
            var data = new SaveData
            {
                totalCoins = TotalCoins,
                currentLevel = isNewGame ? gameConfig.firstLevelSceneIndex : SceneManager.GetActiveScene().buildIndex
            };
            try { File.WriteAllText(SavePath, JsonUtility.ToJson(data)); }
            catch (System.Exception e) { Debug.LogError($"Failed to save: {e.Message}"); }
        }

        void SaveGame() => SaveGame(false);
        public bool SaveDataExists() => File.Exists(SavePath);

        public SaveData LoadGame()
        {
            if (!File.Exists(SavePath)) return null;
            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
                if (data != null)
                {
                    TotalCoins = data.totalCoins;
                    EventBus.OnScoreChanged?.Invoke(TotalCoins);
                }
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load: {e.Message}");
                return null;
            }
        }

        public int GetLastSavedLevelIndex()
        {
            if (!File.Exists(SavePath)) return gameConfig.firstLevelSceneIndex;
            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
                return data?.currentLevel ?? gameConfig.firstLevelSceneIndex;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load: {e.Message}");
                return gameConfig.firstLevelSceneIndex;
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            ResetCoins();
        }

        public void PlaySound(AudioClip sound) 
        { 
            if (sound != null) soundSource.PlayOneShot(sound); 
        }

        public void PlayMusic(AudioClip music, bool loop = true)
        {
            if (music == null) return;
            musicSource.clip = music;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic() => musicSource.Stop();

        public void SetSoundVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            soundSource.volume = volume;
            PlayerPrefs.SetFloat(gameConfig.soundVolumeKey, volume);
            OnSoundVolumeChanged?.Invoke(volume);
        }

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            musicSource.volume = volume;
            PlayerPrefs.SetFloat(gameConfig.musicVolumeKey, volume);
            OnMusicVolumeChanged?.Invoke(volume);
        }
    }
}
