using UnityEngine;

namespace Core
{
    public static class RuntimeInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            Physics2D.jobOptions = new PhysicsJobOptions2D { useMultithreading = true, useConsistencySorting = false };
        }
    }
}
