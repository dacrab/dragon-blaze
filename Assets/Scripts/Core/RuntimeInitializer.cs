using UnityEngine;

namespace DragonBlaze.Core
{
    public static class RuntimeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EnableGPUOptimizations();
            EnablePhysicsOptimizations();
        }

        private static void EnableGPUOptimizations()
        {
#if UNITY_6000_0_OR_NEWER
            if (SystemInfo.supportsGPUResidentDrawer)
            {
                QualitySettings.enableGPUResidentDrawer = true;
                Debug.Log("[Optimization] GPU Resident Drawer enabled");
            }
#endif
        }

        private static void EnablePhysicsOptimizations()
        {
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            Time.fixedDeltaTime = 0.02f;
            Physics2D.jobOptions = new PhysicsJobOptions2D
            {
                useMultithreading = true,
                useConsistencySorting = false
            };
            Debug.Log("[Optimization] Physics2D optimizations enabled (FixedUpdate mode, 0.02s timestep, multithreading)");
        }
    }
}
