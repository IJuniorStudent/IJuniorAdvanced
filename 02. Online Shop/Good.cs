namespace OnlineShop;

public class Good
{
    private string _name;
    
    public Good(string name)
    {
        _name = name;
    }
    
    public string Id => _name;
    public string Name => _name;
}