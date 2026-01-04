using UnityEngine;

namespace Gameplay.Characters.Player
{
    public class PlayerPowerups : MonoBehaviour
    {
        private PlayerVisuals visuals;
        private bool isInvisible;
        public bool IsInvisible => isInvisible;

        private void Awake()
        {
            visuals = GetComponent<PlayerVisuals>();
        }

        public void SetInvisible(bool invisible)
        {
            isInvisible = invisible;
            visuals?.SetInvisibility(invisible);
        }
    }
}
