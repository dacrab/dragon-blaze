namespace Core.Services
{
    /// <summary>Scene navigation contract. Implemented by the persistent loading service in the UI layer
    /// so gameplay and environment code can request a transition without referencing UI.</summary>
    public interface ISceneLoader
    {
        void LoadNextLevel();
        void LoadScene(string? sceneName);
    }
}
