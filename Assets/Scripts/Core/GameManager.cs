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
    public static GameManager instance;

    public delegate void ScoreChanged(int newScore);
    public static event ScoreChanged OnScoreChanged;

    private int totalCoins = 0;
    private string saveFilePath;

    public int TotalCoins
    {
        get { return totalCoins; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This will keep the GameManager alive across different scenes.
        }
        else if (instance != this)
        {
            Destroy(gameObject); // This ensures that there aren't multiple GameManager instances.
        }
        saveFilePath = Application.persistentDataPath + "/savefile.json";
        LoadGame();
    }

    public void AddCoins(int value)
    {
        totalCoins += value;
        OnScoreChanged?.Invoke(totalCoins);
        SaveGame();
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
            uiManager.UpdateCoinDisplay(totalCoins);
        else
            Debug.LogError("UIManager not found.");
    }

    public void ResetCoins()
    {
        totalCoins = 0;
        OnScoreChanged?.Invoke(totalCoins);
        SaveGame();
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
            uiManager.UpdateCoinDisplay(totalCoins);
        else
            Debug.LogError("UIManager not found.");
    }

    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            totalCoins = totalCoins,
            currentLevel = SceneManager.GetActiveScene().buildIndex
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
}
