using UnityEngine;

namespace Gameplay.Characters.Enemies
{
    /// <summary>
    /// Single owner of enemy sprite facing. Both patrol and chase write through here so they can never
    /// fight over <c>localScale</c>, and a zero horizontal delta keeps the current facing instead of
    /// snapping to "right" the way <see cref="Mathf.Sign"/> would.
    /// </summary>
    public readonly struct SpriteFacing
    {
        readonly Vector3 baseScale;

        public SpriteFacing(Vector3 baseScale) => this.baseScale = baseScale;

        public void Apply(Transform target, float deltaX)
        {
            if (Mathf.Approximately(deltaX, 0f)) return;
            float dir = deltaX > 0f ? 1f : -1f;
            target.localScale = new(Mathf.Abs(baseScale.x) * dir, baseScale.y, baseScale.z);
        }
    }
}
