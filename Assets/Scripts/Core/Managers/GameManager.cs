using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Persistence;
using Core.Events;
using UI.Managers;

namespace Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager instance;

        private void Awake()
        {
            InitializeSingleton();
            LoadGame();
        }

        private void InitializeSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Properties
        private int totalCoins = 0;
        public int TotalCoins => totalCoins;
        #endregion

        #region Coin Management
        public void AddCoins(int value)
        {
            totalCoins += value;
            // Notify via EventBus
            EventBus.RaiseScoreChanged(totalCoins);
            
            SaveGame();
            UpdateUICoins();
        }

        public void ResetCoins()
        {
            totalCoins = 0;
            EventBus.RaiseScoreChanged(totalCoins);
            
            SaveGame();
            UpdateUICoins();
        }

        private void UpdateUICoins()
        {
            // Deprecated direct call, kept for safety until UI is refactored
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
                uiManager.UpdateCoinDisplay(totalCoins);
        }
        #endregion

        #region Save/Load System
        public void SaveGame(bool isNewGame = false)
        {
            SaveData data = new SaveData
            {
                totalCoins = totalCoins,
                currentLevel = isNewGame ? 1 : SceneManager.GetActiveScene().buildIndex
            };
            SaveSystem.SaveGame(data);
            EventBus.RaiseGameSaved();
        }

        public bool SaveDataExists()
        {
            return SaveSystem.SaveExists();
        }

        public SaveData LoadGame()
        {
            SaveData data = SaveSystem.LoadGame();
            if (data != null)
            {
                totalCoins = data.totalCoins;
            }
            return data;
        }

        public int GetLastSavedLevelIndex()
        {
            SaveData saveData = SaveSystem.LoadGame();
            return saveData != null ? saveData.currentLevel : 1;
        }
        #endregion
    }
}
