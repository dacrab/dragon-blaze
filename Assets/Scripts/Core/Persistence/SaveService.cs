using System;
using System.IO;
using UnityEngine;

namespace Core.Persistence
{
    /// <summary>
    /// JSON save file. Writes go to a temp file and are then moved into place, so a crash mid-write can
    /// never leave a truncated save that still looks valid to <see cref="SaveDataExists"/>.
    /// Load() returns null when the file is missing or unreadable.
    /// </summary>
    public sealed class SaveService
    {
        readonly string path;

        public SaveService(string filePath) => path = filePath;

        public bool SaveDataExists() => File.Exists(path);

        public void Save(SaveData data)
        {
            if (data == null) return;
            string temp = path + ".tmp";
            try
            {
                File.WriteAllText(temp, JsonUtility.ToJson(data));
                // Replace is not atomic-cross-platform; delete-then-move keeps the steady state consistent.
                if (File.Exists(path)) File.Delete(path);
                File.Move(temp, path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Save failed: {e.Message}");
                TryDelete(temp);
            }
        }

        public SaveData? Load()
        {
            if (!File.Exists(path)) return null;
            try { return JsonUtility.FromJson<SaveData>(File.ReadAllText(path)); }
            catch (Exception e) { Debug.LogError($"[SaveService] Load failed: {e.Message}"); return null; }
        }

        static void TryDelete(string file)
        {
            try { if (File.Exists(file)) File.Delete(file); }
            catch (Exception e) { Debug.LogWarning($"[SaveService] Could not remove temp file: {e.Message}"); }
        }
    }
}
