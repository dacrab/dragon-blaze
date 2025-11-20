using UnityEngine;
using System.Collections;

namespace Gameplay.Characters.Player
{
    public class PlayerPowerups : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private PlayerConfigSO config;

        [Header("Invisibility")]
        [SerializeField] private float defaultInvisibilityDuration = 5f;
        
        [Header("Stats")]
        [SerializeField] private float defaultJumpMultiplier = 1.5f;
        [SerializeField] private float defaultSpeedBoostMultiplier = 1.5f;
        [SerializeField] private float defaultSpeedBoostDuration = 5f;

        private PlayerController controller;
        private PlayerLocomotion locomotion;
        private PlayerVisuals visuals;

        private bool isInvisible;
        public bool IsInvisible => isInvisible;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            locomotion = GetComponent<PlayerLocomotion>();
            visuals = GetComponent<PlayerVisuals>();
        }

        public void ApplyInvisibility(float? duration = null)
        {
            StartCoroutine(InvisibilityCoroutine(duration ?? defaultInvisibilityDuration));
        }

        public void ApplyHigherJump(float multiplier)
        {
            StartCoroutine(HigherJumpCoroutine(multiplier));
        }
        
        // Overload or alternate method if we want to use default settings from just a trigger
        public void ApplyHigherJump()
        {
             StartCoroutine(HigherJumpCoroutine(defaultJumpMultiplier));
        }

        public void ApplySpeedBoost(float? multiplier = null, float? duration = null)
        {
            StartCoroutine(SpeedBoostCoroutine(multiplier ?? defaultSpeedBoostMultiplier, duration ?? defaultSpeedBoostDuration));
        }

        private IEnumerator InvisibilityCoroutine(float duration)
        {
            isInvisible = true;
            visuals.SetInvisibility(true);
            yield return new WaitForSeconds(duration);
            isInvisible = false;
            visuals.SetInvisibility(false);
        }

        private IEnumerator HigherJumpCoroutine(float multiplier)
        {
            float originalJumpPower = locomotion.GetJumpPower();
            // Note: Locomotion now uses config, but 'SetJumpPower' overrides local runtime var
            locomotion.SetJumpPower(originalJumpPower * multiplier);
            yield return new WaitForSeconds(defaultSpeedBoostDuration); // Should be jump boost duration, usually same or separate
            locomotion.SetJumpPower(originalJumpPower);
        }

        private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
        {
            float originalSpeed = locomotion.GetSpeed();
            locomotion.SetSpeed(originalSpeed * multiplier);
            yield return new WaitForSeconds(duration);
            locomotion.SetSpeed(originalSpeed);
        }
    }
}
