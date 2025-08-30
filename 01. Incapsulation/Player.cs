namespace Incapsulation;

public class Player : IDamageable
{
    private int _health;
    
    public Player(int health)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(health);
        
        _health = health;
    }
    
    public event Action<int, int>? HealthChanged;
    
    public bool IsAlive => _health > 0;

    public void TakeDamage(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        
        int oldHealthValue = _health;
        
        _health -= Math.Min(amount, _health);
        
        HealthChanged?.Invoke(oldHealthValue, _health);
    }
}