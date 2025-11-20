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
            try
            {
                string json = JsonUtility.ToJson(data);
                File.WriteAllText(SavePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public static SaveData LoadGame()
        {
            if (!File.Exists(SavePath)) return null;

            try
            {
                string json = File.ReadAllText(SavePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }

        public static bool SaveExists()
        {
            return File.Exists(SavePath);
        }
        
        public static void DeleteSave()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
        }
    }
}
