using UnityEngine;
using System.Collections.Generic;

namespace Core.Analytics
{
    /// <summary>
    /// Base class for analytics backends. Create implementations for Unity Analytics, Firebase, custom, etc.
    /// </summary>
    public abstract class AnalyticsBackendSO : ScriptableObject
    {
        public abstract void SendEvent(string eventName, Dictionary<string, object> parameters);
        public virtual void Initialize() { }
        public virtual void Flush() { }
    }

    /// <summary>
    /// Debug backend that logs to console. Use during development.
    /// </summary>
    [CreateAssetMenu(fileName = "DebugAnalytics", menuName = "DragonBlaze/Analytics/Debug Backend")]
    public sealed class DebugAnalyticsBackend : AnalyticsBackendSO
    {
        public override void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            var paramStr = parameters != null ? string.Join(", ", parameters) : "none";
            UnityEngine.Debug.Log($"[Analytics] {eventName} | {paramStr}");
        }
    }
}
