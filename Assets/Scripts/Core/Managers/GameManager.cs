using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Persistence;
using Core.Events;
using Core.Constants;

namespace Core.Managers
{
    public sealed class GameManager : SingletonManager<GameManager>
    {
        [SerializeField] GameConfig gameConfig;
        SaveSystem saveSystem;

        public int TotalCoins { get; private set; }

        protected override void OnInit()
        {
            if (gameConfig == null) gameConfig = Resources.Load<GameConfig>("GameConfig");
            if (gameConfig == null)
            {
                Debug.LogWarning("GameConfig not found. Creating default settings.");
                gameConfig = ScriptableObject.CreateInstance<GameConfig>();
            }
            
            saveSystem = new SaveSystem(gameConfig);
            LoadGame();
            EventBus.OnLevelCompleted += SaveGame;
        }

        protected override void OnDestroy()
        {
            EventBus.OnLevelCompleted -= SaveGame;
            base.OnDestroy();
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
            saveSystem.SaveGame(new()
            {
                totalCoins = TotalCoins,
                currentLevel = isNewGame ? gameConfig.firstLevelSceneIndex : SceneManager.GetActiveScene().buildIndex
            });
        }

        void SaveGame() => SaveGame(false);
        public bool SaveDataExists() => saveSystem.SaveExists();

        public SaveData LoadGame()
        {
            var data = saveSystem.LoadGame();
            if (data != null)
            {
                TotalCoins = data.totalCoins;
                EventBus.OnScoreChanged?.Invoke(TotalCoins);
            }
            return data;
        }

        public int GetLastSavedLevelIndex() => saveSystem.LoadGame()?.currentLevel ?? gameConfig.firstLevelSceneIndex;

        public void DeleteSave()
        {
            saveSystem.DeleteSave();
            ResetCoins();
        }
    }
}