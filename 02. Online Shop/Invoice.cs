namespace OnlineShop;

public struct Invoice
{
    public Invoice(string payLink)
    {
        Paylink = payLink;
    }
    
    public string Paylink { get; }
}