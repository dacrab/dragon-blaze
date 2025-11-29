using UnityEngine;

namespace Gameplay.Characters.Player
{
    public class PlayerPowerups : MonoBehaviour
    {
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

        public void SetInvisible(bool invisible)
        {
            isInvisible = invisible;
            visuals?.SetInvisibility(invisible);
        }
    }
}
