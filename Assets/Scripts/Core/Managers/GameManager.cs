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
    public sealed class GameManager : MonoBehaviour, IGameManager
    {
        static GameManager instance;

        SaveService saveService;

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

        void OnLevelCompleted(LevelCompletedEvent _) => SaveGame(false);

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
            var data = new SaveData
            {
                totalCoins = TotalCoins,
                levelName = isNewGame ? GameConfig.Default.FirstLevelSceneName : SceneManager.GetActiveScene().name
            };
            saveService.Save(data);
        }

        public bool SaveDataExists() => saveService.SaveDataExists();

        public SaveData LoadGame()
        {
            var data = saveService.Load();
            if (data == null) return null;
            TotalCoins = data.totalCoins;
            EventBus.Raise(new ScoreChangedEvent(TotalCoins));
            return data;
        }

        public string GetLastSavedLevelName() =>
            saveService.Load()?.levelName ?? GameConfig.Default.FirstLevelSceneName;
    }
}
