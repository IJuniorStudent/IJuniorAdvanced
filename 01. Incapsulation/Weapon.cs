namespace Incapsulation;

public class Weapon
{
    private readonly int _damage;
    private readonly int _bullets;
    
    public Weapon(int damage, int bullets)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(damage, nameof(damage));
        ArgumentOutOfRangeException.ThrowIfNegative(bullets, nameof(bullets));
        
        _damage = damage;
        _bullets = bullets;
    }
    
    public bool HasBullets => _bullets > 0;
    
    public void Fire(IDamageable damageable)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_bullets, nameof(_bullets));
        
        damageable.TakeDamage(_damage);
    }
}