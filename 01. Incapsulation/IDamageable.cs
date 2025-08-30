namespace Incapsulation;

public interface IDamageable
{
    public bool IsAlive { get; }
    public void TakeDamage(int amount);
}