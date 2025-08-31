namespace OnlineShop;

public class Cart : IDisposable
{
    private readonly Warehouse _warehouse;
    private readonly InvoiceGenerator _generator;
    private readonly GoodCollection _goods;
    
    public Cart(Warehouse warehouse, InvoiceGenerator generator)
    {
        _warehouse = warehouse;
        _generator = generator;
        _goods = new GoodCollection();
    }
    
    public void Add(Good good, int amount)
    {
        _warehouse.Take(good, amount);
        _goods.Add(new GoodContainer(good, amount));
    }
    
    public Invoice Order()
    {
        _goods.Clear();
        return _generator.Create();
    }
    
    public void Dispose()
    {
        _goods.ForEach((container) =>
        {
            _goods.Take(container.Good, container.Amount);
            _warehouse.Delive(container.Good, container.Amount);
        });
        
        _goods.Clear();
    }
    
    public void Print(IPrinter printer)
    {
        printer.Print("Товары в корзине:");
        _goods.Print(printer);
    }
}