namespace Core.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        bool IsDead { get; }
    }
}
