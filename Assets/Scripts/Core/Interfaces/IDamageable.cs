namespace Core.Interfaces
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        void TakeDamage(float damage);
        void Heal(float amount);
    }
}
}