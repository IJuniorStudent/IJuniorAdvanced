namespace OnlineShop;

public class WebInvoiceGenerator : IInvoiceGenerator
{
    private int _invoiceNumber = 1;
    
    public Invoice Create()
    {
        return new Invoice($"http://prodam.garazh/pay?id={_invoiceNumber++}");
    }
}