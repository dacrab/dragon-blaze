using System;

namespace Core.Persistence
{
    [System.Serializable]
    public sealed class SaveData
    {
        public int version = SaveService.CurrentVersion;
        public int totalCoins;
        public string levelName;
    }
}
