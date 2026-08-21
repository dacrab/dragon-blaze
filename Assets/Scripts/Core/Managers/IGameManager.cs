using Core.Persistence;

namespace Core.Managers
{
    /// <summary>Contract implemented by the persistent GameManager.</summary>
    public interface IGameManager
    {
        int TotalCoins { get; }

        void AddCoins(int value);
        void ResetCoins();
        void SaveGame(bool isNewGame = false);
        bool SaveDataExists();
        SaveData LoadGame();
        string GetLastSavedLevelName();
    }
}
