using UnityEngine;
using System.IO;
using Core.Constants;

namespace Core.Persistence;

[System.Serializable]
public sealed class SaveData
{
    public int totalCoins;
    public int currentLevel;
}

public interface ISaveSystem
{
    void SaveGame(SaveData data);
    SaveData LoadGame();
    bool SaveExists();
    void DeleteSave();
}

public class SaveSystem : ISaveSystem
{
    private readonly GameConfig config;
    private string SavePath => Application.persistentDataPath + "/" + config.saveFileName;

    public SaveSystem(GameConfig gameConfig)
    {
        config = gameConfig;
    }

    public void SaveGame(SaveData data)
    {
        try { File.WriteAllText(SavePath, JsonUtility.ToJson(data)); }
        catch (System.Exception e) { Debug.LogError($"Failed to save: {e.Message}"); }
    }

    public SaveData LoadGame()
    {
        if (!File.Exists(SavePath)) return null;
        try { return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath)); }
        catch (System.Exception e) { Debug.LogError($"Failed to load: {e.Message}"); return null; }
    }

    public bool SaveExists() => File.Exists(SavePath);
    public void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
