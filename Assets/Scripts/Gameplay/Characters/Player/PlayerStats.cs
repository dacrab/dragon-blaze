using System.Collections.Generic;
using Core.Services;

namespace Gameplay.Characters.Player
{
    /// <summary>Tracks timed stat modifiers and computes their combined factor.</summary>
    public sealed class PlayerStats
    {
        readonly Dictionary<PlayerStat, List<Modifier>> modifiers = new();

        public void Add(PlayerStat stat, float factor, float duration)
        {
            if (factor <= 0) return;
            if (!modifiers.TryGetValue(stat, out var list)) modifiers[stat] = list = new List<Modifier>();
            list.Add(new Modifier { Factor = factor, Remaining = duration });
        }

        public void Tick(float deltaTime)
        {
            if (modifiers.Count == 0) return;
            foreach (var list in modifiers.Values)
                for (int i = list.Count - 1; i >= 0; i--)
                    if ((list[i].Remaining -= deltaTime) <= 0) list.RemoveAt(i);
        }

        public void Clear() => modifiers.Clear();

        public float Factor(PlayerStat stat)
        {
            if (!modifiers.TryGetValue(stat, out var list) || list.Count == 0) return 1f;
            float factor = 1f;
            foreach (var modifier in list) factor *= modifier.Factor;
            return factor;
        }

        sealed class Modifier
        {
            public float Factor;
            public float Remaining;
        }
    }
}
