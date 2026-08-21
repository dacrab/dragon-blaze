using UnityEngine;

namespace Core.Services
{
    /// <summary>Contract implemented by the scene Player component.</summary>
    public interface IPlayer
    {
        Transform Transform { get; }
        bool IsInvisible { get; }
        float Damage { get; }
        void SetInvisibility(bool invisible);
        void AddModifier(PlayerStat stat, float factor, float duration);
    }
}