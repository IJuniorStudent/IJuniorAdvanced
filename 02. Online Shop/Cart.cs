namespace OnlineShop;

public class Cart
{
    private readonly IGoodsProvider _goodsProvider;
    private readonly IInvoiceGenerator _invoiceGenerator;
    private readonly GoodCollection _goodsCollection;
    
    public Cart(IGoodsProvider goodsProvider, IInvoiceGenerator invoiceGenerator)
    {
        _goodsProvider = goodsProvider;
        _invoiceGenerator = invoiceGenerator;
        _goodsCollection = new GoodCollection();
    }
    
    public void Add(Good good, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
        
        int goodsAmountLeft = _goodsProvider.GetGoodAmount(good) - _goodsCollection.GetGoodAmount(good);
        
        if (amount > goodsAmountLeft)
            throw new InvalidOperationException($"Trying to take {amount} \"{good.Name}\" of maximum {goodsAmountLeft}");
        
        _goodsCollection.Add(new GoodContainer(good, amount));
    }
    
    public Invoice Order()
    {
        if (_goodsCollection.Count == 0)
            throw new InvalidOperationException("Cart is empty");
        
        _goodsProvider.Take(_goodsCollection.Goods);
        _goodsCollection.Clear();
        
        return _invoiceGenerator.Create();
    }
    
    public void Print(IPrinter printer)
    {
        printer.Print("Товары в корзине:");
        _goodsCollection.Print(printer);
    }
}