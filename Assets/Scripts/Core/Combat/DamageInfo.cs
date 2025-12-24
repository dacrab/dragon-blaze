using UnityEngine;
using Core.Constants;

namespace Core.Combat
{
    /// <summary>
    /// Encapsulates damage information for the combat system.
    /// Allows for damage modifiers, types, and source tracking.
    /// </summary>
    public struct DamageInfo
    {
        /// <summary>
        /// Base damage amount before modifiers.
        /// </summary>
        public float BaseDamage { get; }

        /// <summary>
        /// Type of damage being dealt.
        /// </summary>
        public DamageType DamageType { get; }

        /// <summary>
        /// The GameObject that caused this damage (can be null).
        /// </summary>
        public GameObject Source { get; }

        /// <summary>
        /// Position where the damage originated.
        /// </summary>
        public Vector3 HitPoint { get; }

        /// <summary>
        /// Direction of the attack (for knockback calculations).
        /// </summary>
        public Vector3 HitDirection { get; }

        /// <summary>
        /// Whether this damage can be blocked/parried.
        /// </summary>
        public bool IsBlockable { get; }

        /// <summary>
        /// Whether this damage ignores invulnerability frames.
        /// </summary>
        public bool IgnoresIFrames { get; }

        /// <summary>
        /// Damage multiplier (default 1.0).
        /// </summary>
        public float Multiplier { get; }

        /// <summary>
        /// Final calculated damage after multipliers.
        /// </summary>
        public float FinalDamage => BaseDamage * Multiplier;

        public DamageInfo(
            float baseDamage,
            DamageType damageType = DamageType.Physical,
            GameObject source = null,
            Vector3 hitPoint = default,
            Vector3 hitDirection = default,
            bool isBlockable = true,
            bool ignoresIFrames = false,
            float multiplier = 1f)
        {
            BaseDamage = baseDamage;
            DamageType = damageType;
            Source = source;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
            IsBlockable = isBlockable;
            IgnoresIFrames = ignoresIFrames;
            Multiplier = multiplier;
        }

        /// <summary>
        /// Creates a simple physical damage instance.
        /// </summary>
        public static DamageInfo Physical(float damage, GameObject source = null)
        {
            return new DamageInfo(damage, DamageType.Physical, source);
        }

        /// <summary>
        /// Creates a fire damage instance.
        /// </summary>
        public static DamageInfo Fire(float damage, GameObject source = null)
        {
            return new DamageInfo(damage, DamageType.Fire, source);
        }

        /// <summary>
        /// Creates a magic damage instance.
        /// </summary>
        public static DamageInfo Magic(float damage, GameObject source = null)
        {
            return new DamageInfo(damage, DamageType.Magic, source);
        }

        /// <summary>
        /// Creates damage that ignores invulnerability frames (e.g., environmental hazards).
        /// </summary>
        public static DamageInfo Hazard(float damage, DamageType type = DamageType.Physical)
        {
            return new DamageInfo(damage, type, ignoresIFrames: true, isBlockable: false);
        }
    }
}
