public interface IDamageable
{
    int CurrentHP { get; set; }
    int CurrentShield { get; set; }

    void TakeDamage(int amount);
    void AddShield(int amount);
    void Heal(int amount);
    void Die();
}

