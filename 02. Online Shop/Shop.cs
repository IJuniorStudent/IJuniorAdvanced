namespace OnlineShop;

public class Shop
{
    private readonly Warehouse _warehouse;
    private readonly InvoiceGenerator _generator;
    
    public Shop(Warehouse warehouse)
    {
        _warehouse = warehouse;
        _generator = new InvoiceGenerator();
    }
    
    public Cart Cart()
    {
        return new Cart(_warehouse, _generator);
    }
}
