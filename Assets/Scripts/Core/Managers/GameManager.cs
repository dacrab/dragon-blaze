using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Persistence;
using Core.Events;
using Core.Services;
using Core.State;

namespace Core.Managers
{
    /// <summary>
    /// Main game manager handling game state, coins, and save/load operations.
    /// </summary>
    public class GameManager : SingletonManager<GameManager>
    {
        #region Properties
        private int totalCoins = 0;
        public int TotalCoins => totalCoins;
        #endregion

        #region Initialization
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            // Register as service
            ServiceLocator.Register<GameManager>(this);
            
            // Load game data
            LoadGame();
            
            // Subscribe to events
            EventBus.OnGameStarted += HandleGameStarted;
            EventBus.OnLevelCompleted += HandleLevelCompleted;
        }

        protected override void OnShutdown()
        {
            // Unsubscribe from events
            EventBus.OnGameStarted -= HandleGameStarted;
            EventBus.OnLevelCompleted -= HandleLevelCompleted;
            
            // Unregister service
            ServiceLocator.Unregister<GameManager>();
            
            base.OnShutdown();
        }
        #endregion

        #region Coin Management
        /// <summary>
        /// Adds coins to the total and saves the game.
        /// </summary>
        public void AddCoins(int value)
        {
            if (value <= 0) return;
            
            totalCoins += value;
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }

        /// <summary>
        /// Resets coins to zero and saves the game.
        /// </summary>
        public void ResetCoins()
        {
            totalCoins = 0;
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }

        /// <summary>
        /// Sets coins to a specific value and saves the game.
        /// </summary>
        public void SetCoins(int value)
        {
            totalCoins = Mathf.Max(0, value);
            EventBus.RaiseScoreChanged(totalCoins);
            SaveGame();
        }
        #endregion

        #region Save/Load System
        /// <summary>
        /// Saves the current game state.
        /// </summary>
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

        /// <summary>
        /// Checks if save data exists.
        /// </summary>
        public bool SaveDataExists()
        {
            return SaveSystem.SaveExists();
        }

        /// <summary>
        /// Loads game data from disk.
        /// </summary>
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

        /// <summary>
        /// Gets the last saved level index.
        /// </summary>
        public int GetLastSavedLevelIndex()
        {
            SaveData saveData = SaveSystem.LoadGame();
            return saveData != null ? saveData.currentLevel : Core.Constants.GameConstants.Scenes.FirstLevel;
        }

        /// <summary>
        /// Deletes the save file.
        /// </summary>
        public void DeleteSave()
        {
            SaveSystem.DeleteSave();
            ResetCoins();
        }
        #endregion

        #region Event Handlers
        private void HandleGameStarted()
        {
            // Game started logic
        }

        private void HandleLevelCompleted()
        {
            SaveGame();
        }
        #endregion
    }
}
