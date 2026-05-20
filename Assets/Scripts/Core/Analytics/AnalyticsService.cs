using UnityEngine;
using System.Collections.Generic;

namespace Core.Analytics
{
    /// <summary>
    /// Central analytics dispatcher. Assign a backend via AnalyticsConfig SO.
    /// Automatically hooks into EventBus for common game events.
    /// </summary>
    [CreateAssetMenu(fileName = "AnalyticsConfig", menuName = "DragonBlaze/Analytics/Config")]
    public sealed class AnalyticsConfigSO : ScriptableObject
    {
        public AnalyticsBackendSO backend;
        public bool enabled = true;
    }

    public static class AnalyticsService
    {
        static AnalyticsConfigSO config;

        public static void Initialize(AnalyticsConfigSO cfg)
        {
            config = cfg;
            config?.backend?.Initialize();
        }

        public static void Send(string eventName, Dictionary<string, object> parameters = null)
        {
            if (config == null || !config.enabled || config.backend == null) return;
            config.backend.SendEvent(eventName, parameters);
        }

        public static void Flush() => config?.backend?.Flush();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => config = null;
    }
}
