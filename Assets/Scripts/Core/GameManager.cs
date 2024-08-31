using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public int totalCoins;
    public int currentLevel;
}

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        InitializeSingleton();
        InitializeSaveSystem();
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

    private void InitializeSaveSystem()
    {
        saveFilePath = Application.persistentDataPath + "/savefile.json";
        LoadGame();
    }
    #endregion

    #region Properties
    private int totalCoins = 0;
    public int TotalCoins => totalCoins;

    private string saveFilePath;
    #endregion

    #region Events
    public delegate void ScoreChanged(int newScore);
    public static event ScoreChanged OnScoreChanged;
    #endregion

    #region Coin Management
    public void AddCoins(int value)
    {
        totalCoins += value;
        OnScoreChanged?.Invoke(totalCoins);
        SaveGame();
        UpdateUICoins();
    }

    public void ResetCoins()
    {
        totalCoins = 0;
        OnScoreChanged?.Invoke(totalCoins);
        SaveGame();
        UpdateUICoins();
    }

    private void UpdateUICoins()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
            uiManager.UpdateCoinDisplay(totalCoins);
        else
            Debug.LogError("UIManager not found.");
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
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public bool SaveDataExists()
    {
        return File.Exists(saveFilePath);
    }

    public SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            totalCoins = data.totalCoins;
            return data;
        }
        return null;
    }

    public int GetLastSavedLevelIndex()
    {
        SaveData saveData = LoadSaveData();
        return saveData != null ? saveData.currentLevel : 1;
    }

    private SaveData LoadSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return null;
    }
    #endregion
}