using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Persistence;
using Core.Services;

namespace Core.Managers
{
    /// <summary>
    /// Persistent cross-scene state: coins, saves, and last-level tracking.
    /// Self-bootstraps before the first scene so no scene placement is required.
    /// </summary>
    public sealed class GameManager : MonoBehaviour, IGameManager
    {
        static GameManager? instance;

        SaveService? saveService;

        /// <summary>Level to resume from "Continue"; excludes the menu and the credits roll.</summary>
        string? lastLevel;

        public int TotalCoins { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (instance != null) return;
            _ = new GameObject(nameof(GameManager)).AddComponent<GameManager>();
        }

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveService = new SaveService(Path.Combine(Application.persistentDataPath, GameConfig.Default.saveFileName));
            ServiceLocator.Register<IGameManager>(this);
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
            LoadGame();
        }

        void OnDestroy()
        {
            if (instance != this) return;
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            ServiceLocator.Unregister<IGameManager>();
            instance = null;
        }

        void OnApplicationQuit()
        {
            if (instance == this) SaveGame();
        }

        void OnLevelCompleted(LevelCompletedEvent _)
        {
            // Remember the level just finished before the credits take over the active scene.
            lastLevel = SceneManager.GetActiveScene().name;
            SaveGame(false);
        }

        public void AddCoins(int value)
        {
            if (value <= 0) return;
            TotalCoins += value;
            EventBus.Raise(new ScoreChangedEvent(TotalCoins));
        }

        public void ResetCoins()
        {
            TotalCoins = 0;
            EventBus.Raise(new ScoreChangedEvent(TotalCoins));
        }

        public void SaveGame(bool isNewGame = false)
        {
            // Keep the last real level: quitting from the menu or credits must not break "Continue".
            if (isNewGame) lastLevel = GameConfig.Default.FirstLevelSceneName;
            else lastLevel = PlayableLevelOr(SceneManager.GetActiveScene().name) ?? lastLevel;

            saveService?.Save(new SaveData { totalCoins = TotalCoins, levelName = lastLevel });
        }

        public bool SaveDataExists() => saveService != null && saveService.SaveDataExists();

        public SaveData? LoadGame()
        {
            if (saveService == null) return null;
            var data = saveService.Load();
            if (data == null) return null;
            TotalCoins = Mathf.Max(0, data.totalCoins);
            lastLevel = data.levelName;
            EventBus.Raise(new ScoreChangedEvent(TotalCoins));
            return data;
        }

        public string GetLastSavedLevelName()
        {
            return PlayableLevelOr(lastLevel) ?? GameConfig.Default.FirstLevelSceneName ?? string.Empty;
        }

        /// <summary>Returns <paramref name="levelName"/> when it is a real playable level, otherwise null.</summary>
        static string? PlayableLevelOr(string? levelName)
        {
            if (string.IsNullOrEmpty(levelName)) return null;
            var config = GameConfig.Default;
            var order = config.levelOrder;
            // Index 0 is the main menu; resuming into it would be useless.
            int index = Array.IndexOf(order, levelName);
            if (index <= 0) return null;
            // Credits/outro scenes sort after the levels but are not resumable.
            return config.IsMenuScene(levelName) ? null : levelName;
        }
    }
}
