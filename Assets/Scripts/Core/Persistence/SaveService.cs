using System;
using System.IO;
using UnityEngine;

namespace Core.Persistence
{
    /// <summary>
    /// Versioned JSON save file. Saves are written to a single path; Load() returns null when
    /// the file is missing or unreadable. Bump CurrentVersion and extend Migrate() when the
    /// save shape evolves.
    /// </summary>
    public sealed class SaveService
    {
        public const int CurrentVersion = 1;

        readonly string path;

        public SaveService(string filePath) => path = filePath;

        public bool SaveDataExists() => File.Exists(path);

        public void Save(SaveData data)
        {
            if (data == null) return;
            data.version = CurrentVersion;
            try { File.WriteAllText(path, JsonUtility.ToJson(data)); }
            catch (Exception e) { Debug.LogError($"[SaveService] Save failed: {e.Message}"); }
        }

        public SaveData Load()
        {
            if (!File.Exists(path)) return null;
            try { return Migrate(JsonUtility.FromJson<SaveData>(File.ReadAllText(path))); }
            catch (Exception e) { Debug.LogError($"[SaveService] Load failed: {e.Message}"); return null; }
        }

        static SaveData Migrate(SaveData data)
        {
            if (data == null) return null;
            if (data.version < CurrentVersion) data.version = CurrentVersion;
            // Future migrations: chain per-version upgrades here before returning.
            return data;
        }
    }
}
