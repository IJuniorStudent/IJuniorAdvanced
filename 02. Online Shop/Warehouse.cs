namespace OnlineShop;

public class Warehouse : IGoodsProvider
{
    private GoodCollection _goodsCollection;
    
    public Warehouse()
    {
        _goodsCollection = new GoodCollection();
    }
    
    public void Delive(Good good, int amount)
    {
        _goodsCollection.Add(new GoodContainer(good, amount));
    }
    
    public void Print(IPrinter printer)
    {
        printer.Print("Товары на складе:");
        _goodsCollection.Print(printer);
    }
    
    public int GetGoodAmount(Good good)
    {
        return _goodsCollection.GetGoodAmount(good);
    }
    
    public void Take(IReadOnlyList<IReadOnlyGoodContainer> goods)
    {
        if (CanTake(goods) == false)
            throw new InvalidOperationException("Can not take all goods");
        
        foreach (var goodContainer in goods)
            _goodsCollection.Take(goodContainer.Good, goodContainer.Amount);
    }
    
    private bool CanTake(IReadOnlyList<IReadOnlyGoodContainer> goods)
    {
        foreach (var goodContainer in goods)
        {
            if (_goodsCollection.GetGoodAmount(goodContainer.Good) < goodContainer.Amount)
                return false;
        }
        
        return true;
    }
}
