using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Constants;
using Core.Events;
using Core.Managers;
using Core.Services;
using Core.State;

namespace UI.Managers
{
    public sealed partial class UIManager
    {
        public void NewGame() => StartNewGame();

        public static void StartNewGame()
        {
            var gameManager = ServiceLocator.Get<IGameManager>();
            gameManager?.ResetCoins();
            gameManager?.SaveGame(true);
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.FirstLevelSceneName);
        }

        public void ContinueGame() =>
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(
                ServiceLocator.Get<IGameManager>()?.GetLastSavedLevelName() ?? GameConfig.Default.FirstLevelSceneName);

        public void GameOver(PlayerDiedEvent _)
        {
            if (gameOverScreen == null || gameOverScreen.activeInHierarchy) return;
            // Game over owns the screen: leaving pause open stacks two overlays and keeps timeScale at 0.
            DismissPause();
            gameOverScreen.SetActive(true);
            SetCursor(true);
            ServiceLocator.Get<IGameStateManager>()?.ChangeState(GameState.GameOver);
            ServiceLocator.Get<IAudioManager>()?.PlaySound(gameOverSound);
        }

        public void Restart()
        {
            var player = ServiceLocator.Get<IPlayer>();
            var stateManager = ServiceLocator.Get<IGameStateManager>();
            // A checkpoint respawn only makes sense from game over; otherwise reload the scene from scratch.
            if (player != null && player.HasCheckpoint() && stateManager?.CurrentState == GameState.GameOver)
            {
                DismissPause();
                gameOverScreen?.SetActive(false);
                EventBus.Raise(new PlayerRespawnEvent());
                SetCursor(false);
                return;
            }
            DismissPause();
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void MainMenu()
        {
            SetCursor(true);
            ServiceLocator.Get<ISceneLoader>()?.LoadScene(GameConfig.Default.MainMenuSceneName);
        }

        public void Quit()
        {
            // GameManager.OnApplicationQuit persists coins/level; saving here too would just write twice.
            SetCursor(true);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void TogglePause()
        {
            if (pauseScreen == null) return;
            // Never let pause resume a death screen (would set timeScale back to 1 behind it).
            if (gameOverScreen is { activeInHierarchy: true }) return;
            if (!pauseScreen.activeInHierarchy) PauseGame();
            else ResumeGame();
        }

        /// <summary>UnityEvent-friendly pause entry point (used by the pause screen button).</summary>
        public void PauseGame()
        {
            // Nothing to pause without a pause screen, or while game over owns the screen.
            if (pauseScreen == null || pauseScreen.activeInHierarchy || gameOverScreen is { activeInHierarchy: true }) return;

            pauseScreen.SetActive(true);
            SetCursor(true);
            EventBus.Raise(new GamePausedEvent(true));
        }

        /// <summary>Closes the pause overlay without touching the game state; used when game over or a scene change takes over.</summary>
        void DismissPause()
        {
            if (pauseScreen != null && pauseScreen.activeInHierarchy) pauseScreen.SetActive(false);
        }

        public void ResumeGame()
        {
            if (pauseScreen == null || !pauseScreen.activeInHierarchy) return;

            pauseScreen.SetActive(false);
            SetCursor(false);
            EventBus.Raise(new GamePausedEvent(false));
        }

        public void ShowLoadingScreen(bool show) => loadingScreen?.SetActive(show);

        public void UpdateLoadingImage(float progress)
        {
            if (loadingImage != null) loadingImage.fillAmount = progress;
        }

        void SetCursor(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
