using UnityEngine;
using Core.Utilities;

namespace Gameplay.Characters.Player
{
    public class PlayerPowerups : MonoBehaviour
    {
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private PlayerController controller;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private PlayerLocomotion locomotion;
        [AutoWire(AutoWireAttribute.WireType.Self)]
        [SerializeField] private PlayerVisuals visuals;

        private bool isInvisible;
        public bool IsInvisible => isInvisible;

        private void Awake()
        {
            AutoWireHelper.WireAllFields(this);
        }

        public void SetInvisible(bool invisible)
        {
            isInvisible = invisible;
            visuals?.SetInvisibility(invisible);
        }
    }
}
