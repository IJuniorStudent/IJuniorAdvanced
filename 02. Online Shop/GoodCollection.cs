namespace OnlineShop;

public class GoodCollection
{
    private Dictionary<string, GoodContainer> _containers;
    
    public GoodCollection()
    {
        _containers = new Dictionary<string, GoodContainer>();
    }
    
    public void Add(GoodContainer container)
    {
        if (_containers.TryGetValue(container.Good.Id, out GoodContainer? selfContainer))
            selfContainer.Merge(container);
        else
            _containers.Add(container.Good.Id, container);
    }
    
    public void Take(Good good, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
        
        if (_containers.TryGetValue(good.Id, out GoodContainer? container) == false)
            throw new InvalidOperationException($"Good with id {good.Id} not found");
        
        container.Take(amount);
    }
    
    public void Clear()
    {
        _containers.Clear();
    }
    
    public void ForEach(Action<GoodContainer> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        
        foreach (GoodContainer container in _containers.Values)
            action(container);
    }
    
    public void Print(IPrinter printer)
    {
        foreach (GoodContainer container in _containers.Values)
            printer.Print($"{container.Good.Name} - {container.Amount}");
    }
}