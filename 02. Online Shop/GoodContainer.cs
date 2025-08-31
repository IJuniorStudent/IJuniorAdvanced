namespace OnlineShop;

public class GoodContainer : IReadOnlyGoodContainer
{
    private readonly Good _good;
    private int _amount;
    
    public GoodContainer(Good good, int amount)
    {
        ArgumentNullException.ThrowIfNull(good, nameof(good));
        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
        
        _good = good;
        _amount = amount;
    }
    
    public Good Good => _good;
    public int Amount => _amount;
    
    public void Merge(GoodContainer container)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(container.Amount, nameof(container.Amount));
        
        if (container.Good.Id != _good.Id)
            throw new ArgumentException($"Id \"{_good.Id}\" doesn't match income Id \"{container.Good.Id}\"");
        
        _amount += container.Amount;
    }
    
    public void Take(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
        
        if (amount > _amount)
            throw new InvalidOperationException($"Trying to take {amount} of good {_good.Id} that more than exists - {_amount}");
        
        _amount -= amount;
    }
}
