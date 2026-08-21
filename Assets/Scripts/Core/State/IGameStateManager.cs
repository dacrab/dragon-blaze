using Core.Constants;

namespace Core.State
{
    /// <summary>Contract implemented by the persistent GameStateManager.</summary>
    public interface IGameStateManager
    {
        GameState CurrentState { get; }
        bool IsPlaying { get; }
        void ChangeState(GameState state);
    }
}
