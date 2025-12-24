using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Helper class for managing cooldown timers.
    /// Replaces Mathf.Infinity pattern with cleaner API.
    /// </summary>
    public struct CooldownTimer
    {
        private float _cooldown;
        private float _timer;

        /// <summary>
        /// Creates a new cooldown timer with specified duration.
        /// </summary>
        public CooldownTimer(float cooldown)
        {
            _cooldown = cooldown;
            _timer = float.MaxValue; // Start on cooldown (ready to use)
        }

        /// <summary>
        /// Returns true if cooldown has elapsed (ready to use).
        /// </summary>
        public bool IsReady => _timer >= _cooldown;

        /// <summary>
        /// Returns remaining cooldown time (0 if ready).
        /// </summary>
        public float RemainingTime => Mathf.Max(0f, _cooldown - _timer);

        /// <summary>
        /// Updates the timer. Call this in Update().
        /// </summary>
        public void Update()
        {
            _timer += Time.deltaTime;
        }

        /// <summary>
        /// Resets the cooldown timer (starts counting from 0).
        /// </summary>
        public void Reset()
        {
            _timer = 0f;
        }

        /// <summary>
        /// Resets and makes ready immediately.
        /// </summary>
        public void ResetToReady()
        {
            _timer = _cooldown;
        }
    }
}

