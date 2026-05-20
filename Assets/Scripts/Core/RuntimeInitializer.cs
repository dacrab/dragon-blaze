using UnityEngine;

namespace Core
{
    public static class RuntimeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
#if UNITY_6000_0_OR_NEWER
            if (SystemInfo.supportsGPUResidentDrawer)
                QualitySettings.enableGPUResidentDrawer = true;
#endif
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            Physics2D.jobOptions = new PhysicsJobOptions2D { useMultithreading = true, useConsistencySorting = false };
        }
    }
}
