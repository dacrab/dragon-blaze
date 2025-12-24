using Core.State;
using Core.Constants;

namespace Core.Utilities
{
    /// <summary>
    /// Static helper methods for quick game state checks.
    /// Reduces boilerplate when checking game state in gameplay code.
    /// Caches GameStateManager.Instance for performance.
    /// </summary>
    public static class GameStateHelpers
    {
        private static GameStateManager _instance;
        
        private static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameStateManager.Instance;
                }
                return _instance;
            }
        }

        /// <summary>
        /// Returns true if the game is currently in gameplay state.
        /// </summary>
        public static bool IsPlaying => Instance?.IsPlaying ?? true;

        /// <summary>
        /// Returns true if the game is paused.
        /// </summary>
        public static bool IsPaused => Instance?.IsPaused ?? false;

        /// <summary>
        /// Returns true if a dialogue is currently active.
        /// </summary>
        public static bool IsInDialogue => Instance?.IsInDialogue ?? false;

        /// <summary>
        /// Returns true if the game is over.
        /// </summary>
        public static bool IsGameOver => Instance?.IsGameOver ?? false;

        /// <summary>
        /// Returns true if currently in the main menu.
        /// </summary>
        public static bool IsInMenu => Instance?.IsInMenu ?? false;

        /// <summary>
        /// Returns true if a scene is loading.
        /// </summary>
        public static bool IsLoading => Instance?.IsLoading ?? false;

        /// <summary>
        /// Returns true if gameplay input should be processed.
        /// </summary>
        public static bool CanProcessInput => Instance?.CanProcessGameplayInput ?? true;

        /// <summary>
        /// Returns the current game state.
        /// </summary>
        public static GameState CurrentState => Instance?.CurrentState ?? GameState.Gameplay;

        /// <summary>
        /// Checks if the current state matches any of the provided states.
        /// </summary>
        public static bool IsAnyState(params GameState[] states)
        {
            return Instance?.IsAnyState(states) ?? false;
        }
        
        /// <summary>
        /// Clears the cached instance. Call when GameStateManager is destroyed.
        /// </summary>
        internal static void ClearCache()
        {
            _instance = null;
        }
    }
}
