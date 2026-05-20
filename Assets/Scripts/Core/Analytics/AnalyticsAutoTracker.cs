using UnityEngine;
using System.Collections.Generic;
using Core.Events;

namespace Core.Analytics
{
    /// <summary>
    /// Drop on a persistent GameObject. Automatically tracks common events.
    /// </summary>
    public sealed class AnalyticsAutoTracker : MonoBehaviour
    {
        [SerializeField] AnalyticsConfigSO config;

        void Awake() => AnalyticsService.Initialize(config);

        void OnEnable()
        {
            EventBus.OnPlayerDied += OnPlayerDied;
            EventBus.OnLevelLoaded += OnLevelLoaded;
            EventBus.OnLevelCompleted += OnLevelCompleted;
            EventBus.OnScoreChanged += OnScoreChanged;
        }

        void OnDisable()
        {
            EventBus.OnPlayerDied -= OnPlayerDied;
            EventBus.OnLevelLoaded -= OnLevelLoaded;
            EventBus.OnLevelCompleted -= OnLevelCompleted;
            EventBus.OnScoreChanged -= OnScoreChanged;
        }

        void OnPlayerDied() => AnalyticsService.Send("player_died", new Dictionary<string, object>
        {
            { "time_alive", Time.timeSinceLevelLoad }
        });

        void OnLevelLoaded(int index) => AnalyticsService.Send("level_started", new Dictionary<string, object>
        {
            { "level_index", index }
        });

        void OnLevelCompleted() => AnalyticsService.Send("level_completed", new Dictionary<string, object>
        {
            { "completion_time", Time.timeSinceLevelLoad }
        });

        void OnScoreChanged(int score) => AnalyticsService.Send("score_updated", new Dictionary<string, object>
        {
            { "total_score", score }
        });

        void OnApplicationQuit() => AnalyticsService.Flush();
    }
}
