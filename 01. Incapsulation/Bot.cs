namespace Incapsulation;

public class Bot
{
    private readonly Weapon _weapon;
    
    public Bot(Weapon weapon)
    {
        _weapon = weapon;
    }
    
    public void OnSeePlayer(IAttackableTarget target)
    {
        if (_weapon.HasBullets && target.IsAlive())
            _weapon.Fire(target);
    }
}