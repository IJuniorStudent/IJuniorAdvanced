namespace OnlineShop;

public class Shop
{
    private readonly Warehouse _warehouse;
    private readonly WebInvoiceGenerator _generator;
    
    public Shop(Warehouse warehouse)
    {
        _warehouse = warehouse;
        _generator = new WebInvoiceGenerator();
    }
    
    public Cart Cart()
    {
        return new Cart(_warehouse, _generator);
    }
}
