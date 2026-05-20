using UnityEngine;
using System.Collections.Generic;

namespace Core.Analytics
{
    /// <summary>
    /// Define analytics events as assets. Drag into components to fire events without hardcoded strings.
    /// </summary>
    [CreateAssetMenu(fileName = "AnalyticsEvent", menuName = "DragonBlaze/Analytics/Event")]
    public sealed class AnalyticsEventSO : ScriptableObject
    {
        [SerializeField] string eventName;
        [SerializeField] string description;

        public void Send(Dictionary<string, object> parameters = null) =>
            AnalyticsService.Send(eventName, parameters);

        public void Send(string key, object value) =>
            AnalyticsService.Send(eventName, new Dictionary<string, object> { { key, value } });
    }
}
