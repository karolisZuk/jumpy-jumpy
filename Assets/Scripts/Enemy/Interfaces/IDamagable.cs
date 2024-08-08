public interface IDamagable {
    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }

    void Damage(int damageAmount);
    void Die();
}
