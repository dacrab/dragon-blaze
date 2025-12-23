using UnityEngine;
using System.IO;
using Core.Constants;

namespace Core.Persistence
{
    [System.Serializable]
    public class SaveData
    {
        public int totalCoins;
        public int currentLevel;
    }

    public static class SaveSystem
    {
        private static string SavePath => Application.persistentDataPath + GameConstants.Save.SaveFileName;

        public static void SaveGame(SaveData data)
        {
            try { File.WriteAllText(SavePath, JsonUtility.ToJson(data)); }
            catch (System.Exception e) { Debug.LogError($"Failed to save game: {e.Message}"); }
        }

        public static SaveData LoadGame()
        {
            if (!File.Exists(SavePath)) return null;
            try { return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath)); }
            catch (System.Exception e) { Debug.LogError($"Failed to load game: {e.Message}"); return null; }
        }

        public static bool SaveExists() => File.Exists(SavePath);
        public static void DeleteSave() { if (File.Exists(SavePath)) File.Delete(SavePath); }
    }
}
