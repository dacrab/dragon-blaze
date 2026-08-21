using System;
using System.IO;
using UnityEngine;

namespace Core.Persistence
{
    /// <summary>
    /// Simple JSON save file. Saves are written to a single path; Load() returns null when
    /// the file is missing or unreadable.
    /// </summary>
    public sealed class SaveService
    {
        readonly string path;

        public SaveService(string filePath) => path = filePath;

        public bool SaveDataExists() => File.Exists(path);

        public void Save(SaveData data)
        {
            if (data == null) return;
            try { File.WriteAllText(path, JsonUtility.ToJson(data)); }
            catch (Exception e) { Debug.LogError($"[SaveService] Save failed: {e.Message}"); }
        }

        public SaveData Load()
        {
            if (!File.Exists(path)) return null;
            try { return JsonUtility.FromJson<SaveData>(File.ReadAllText(path)); }
            catch (Exception e) { Debug.LogError($"[SaveService] Load failed: {e.Message}"); return null; }
        }
    }
}