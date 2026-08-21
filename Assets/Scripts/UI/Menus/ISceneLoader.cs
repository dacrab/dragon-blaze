namespace UI.Menus
{
    /// <summary>Contract implemented by the persistent LoadingManager.</summary>
    public interface ISceneLoader
    {
        void LoadNextLevel();
        void LoadScene(string sceneName);
    }
}
