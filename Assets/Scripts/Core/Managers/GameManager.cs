using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Persistence;
using Core.Events;

namespace Core.Managers
{
    public class GameManager : SingletonManager<GameManager>
    {
        private int totalCoins;
        public int TotalCoins => totalCoins;

        protected override void OnInitialize()
        {
            LoadGame();
            EventBus.OnGameStarted += HandleGameStarted;
            EventBus.OnLevelCompleted += HandleLevelCompleted;
        }

        protected override void OnShutdown()
        {
            EventBus.OnGameStarted -= HandleGameStarted;
            EventBus.OnLevelCompleted -= HandleLevelCompleted;
        }

        public void AddCoins(int value)
        {
            if (value <= 0) return;
            totalCoins += value;
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }

        public void ResetCoins()
        {
            totalCoins = 0;
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }

        public void SetCoins(int value)
        {
            totalCoins = Mathf.Max(0, value);
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }

        public void SaveGame(bool isNewGame = false)
        {
            SaveData data = new SaveData
            {
                totalCoins = totalCoins,
                currentLevel = isNewGame ? Core.Constants.GameConstants.Scenes.FirstLevel : SceneManager.GetActiveScene().buildIndex
            };
            SaveSystem.SaveGame(data);
            EventBus.RaiseGameSaved();
        }

        public bool SaveDataExists() => SaveSystem.SaveExists();

        public SaveData LoadGame()
        {
            SaveData data = SaveSystem.LoadGame();
            if (data != null)
            {
                totalCoins = data.totalCoins;
                EventBus.RaiseScoreChanged(totalCoins);
            }
            return data;
        }

        public int GetLastSavedLevelIndex()
        {
            SaveData saveData = SaveSystem.LoadGame();
            return saveData != null ? saveData.currentLevel : Core.Constants.GameConstants.Scenes.FirstLevel;
        }

        public void DeleteSave()
        {
            SaveSystem.DeleteSave();
            ResetCoins();
        }

        private void HandleGameStarted() { }
        private void HandleLevelCompleted() => SaveGame();
    }
}
