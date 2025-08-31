namespace OnlineShop;

public class InvoiceGenerator
{
    private int _invoiceNumber = 1;
    
    public Invoice Create()
    {
        return new Invoice($"http://prodam.garazh/pay?id={_invoiceNumber++}");
    }
}