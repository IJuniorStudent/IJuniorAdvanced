namespace OnlineShop;

public class Warehouse
{
    private GoodCollection _goods;
    
    public Warehouse()
    {
        _goods = new GoodCollection();
    }
    
    public void Delive(Good good, int amount)
    {
        _goods.Add(new GoodContainer(good, amount));
    }
    
    public void Take(Good good, int amount)
    {
        _goods.Take(good, amount);
    }
    
    public void Print(IPrinter printer)
    {
        printer.Print("Товары на складе:");
        _goods.Print(printer);
    }
}
