using System;
using Core.Constants;

namespace Core.Events
{

public interface IEventBus
{
    event Action<int> OnScoreChanged;
    event Action OnPlayerDied;
    event Action OnPlayerRespawn;
    event Action<int> OnLevelLoaded;
    event Action<bool> OnGamePaused;
    event Action OnLevelCompleted;
    event Action<GameState> OnGameStateChanged;
    event Action<bool> OnDialogueStateChanged;
    event Action<float, float> OnHealthChanged;

    void ScoreChanged(int score);
    void PlayerDied();
    void PlayerRespawn();
    void LevelLoaded(int level);
    void GamePaused(bool paused);
    void LevelCompleted();
    void GameStateChanged(GameState state);
    void DialogueStateChanged(bool open);
    void HealthChanged(float current, float max);
    void ClearAll();
}

public static class EventBus
{
    private static IEventBus _instance = new EventBusImpl();
    public static IEventBus Instance => _instance;

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() => _instance.ClearAll();

    public static event Action<int> OnScoreChanged { add => _instance.OnScoreChanged += value; remove => _instance.OnScoreChanged -= value; }
    public static event Action OnPlayerDied { add => _instance.OnPlayerDied += value; remove => _instance.OnPlayerDied -= value; }
    public static event Action OnPlayerRespawn { add => _instance.OnPlayerRespawn += value; remove => _instance.OnPlayerRespawn -= value; }
    public static event Action<int> OnLevelLoaded { add => _instance.OnLevelLoaded += value; remove => _instance.OnLevelLoaded -= value; }
    public static event Action<bool> OnGamePaused { add => _instance.OnGamePaused += value; remove => _instance.OnGamePaused -= value; }
    public static event Action OnLevelCompleted { add => _instance.OnLevelCompleted += value; remove => _instance.OnLevelCompleted -= value; }
    public static event Action<GameState> OnGameStateChanged { add => _instance.OnGameStateChanged += value; remove => _instance.OnGameStateChanged -= value; }
    public static event Action<bool> OnDialogueStateChanged { add => _instance.OnDialogueStateChanged += value; remove => _instance.OnDialogueStateChanged -= value; }
    public static event Action<float, float> OnHealthChanged { add => _instance.OnHealthChanged += value; remove => _instance.OnHealthChanged -= value; }

    public static void ScoreChanged(int score) => _instance.ScoreChanged(score);
    public static void PlayerDied() => _instance.PlayerDied();
    public static void PlayerRespawn() => _instance.PlayerRespawn();
    public static void LevelLoaded(int level) => _instance.LevelLoaded(level);
    public static void GamePaused(bool paused) => _instance.GamePaused(paused);
    public static void LevelCompleted() => _instance.LevelCompleted();
    public static void GameStateChanged(GameState state) => _instance.GameStateChanged(state);
    public static void DialogueStateChanged(bool open) => _instance.DialogueStateChanged(open);
    public static void HealthChanged(float current, float max) => _instance.HealthChanged(current, max);
    public static void ClearAll() => _instance.ClearAll();
}

internal class EventBusImpl : IEventBus
{
    public event Action<int> OnScoreChanged;
    public event Action OnPlayerDied;
    public event Action OnPlayerRespawn;
    public event Action<int> OnLevelLoaded;
    public event Action<bool> OnGamePaused;
    public event Action OnLevelCompleted;
    public event Action<GameState> OnGameStateChanged;
    public event Action<bool> OnDialogueStateChanged;
    public event Action<float, float> OnHealthChanged;

    public void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public void PlayerDied() => OnPlayerDied?.Invoke();
    public void PlayerRespawn() => OnPlayerRespawn?.Invoke();
    public void LevelLoaded(int level) => OnLevelLoaded?.Invoke(level);
    public void GamePaused(bool paused) => OnGamePaused?.Invoke(paused);
    public void LevelCompleted() => OnLevelCompleted?.Invoke();
    public void GameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
    public void DialogueStateChanged(bool open) => OnDialogueStateChanged?.Invoke(open);
    public void HealthChanged(float current, float max) => OnHealthChanged?.Invoke(current, max);

    public void ClearAll()
    {
        OnScoreChanged = null;
        OnPlayerDied = null;
        OnPlayerRespawn = null;
        OnLevelLoaded = null;
        OnGamePaused = null;
        OnLevelCompleted = null;
        OnGameStateChanged = null;
        OnDialogueStateChanged = null;
        OnHealthChanged = null;
    }
}
}