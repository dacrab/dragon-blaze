using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Persistence;
using Core.Events;
using UI.Managers;

namespace Core.Managers
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private int totalCoins;
        public int TotalCoins => totalCoins;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadGame();
        }

        public void AddCoins(int value)
        {
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

        public void SaveGame(bool isNewGame = false)
        {
            var data = new SaveData
            {
                totalCoins = totalCoins,
                currentLevel = isNewGame ? 1 : SceneManager.GetActiveScene().buildIndex
            };

            SaveSystem.SaveGame(data);
            EventBus.RaiseGameSaved();
        }

        public bool SaveDataExists() => SaveSystem.SaveExists();

        public SaveData LoadGame()
        {
            var data = SaveSystem.LoadGame();
            if (data != null)
            {
                totalCoins = data.totalCoins;
            }

            return data;
        }

        public int GetLastSavedLevelIndex()
        {
            var saveData = SaveSystem.LoadGame();
            return saveData != null ? saveData.currentLevel : 1;
        }
    }
}
