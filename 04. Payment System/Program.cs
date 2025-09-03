using System.Security.Cryptography;
using System.Text;

namespace PaymentSystem;

class Program
{
    static void Main(string[] args)
    {
        int orderId = 1;
        int orderAmount = 12_000;
        string privateSignKey = "private sign key";
        string defaultCurrency = "RUB";
        
        var order = new Order(orderId, orderAmount);
        
        PaymentSystem[] paymentSystems =
        [
            new (
                "pay.system1.ru/order",
                defaultCurrency,
                new PaymentQueryBuilder1(new OrderIdRetriever(), MD5.Create())
            ),
            
            new (
                "order.system2.ru/pay", 
                defaultCurrency,
                new PaymentQueryBuilder2(new OrderIdAmountRetriever(), MD5.Create())
            ),
            
            new (
                "system3.com/pay", 
                defaultCurrency,
                new PaymentQueryBuilder3(new SecuredOrderRetriever(new OrderIdAmountRetriever(), privateSignKey), SHA1.Create())
            ),
        ];
        
        foreach (var paymentSystem in paymentSystems)
            Console.WriteLine(paymentSystem.GetPayingLink(order));
    }
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id, int amount) => (Id, Amount) = (id, amount);
}

public interface IOrderDataRetriever
{
    public string Get(Order order);
}

public class OrderIdRetriever : IOrderDataRetriever
{
    public string Get(Order order)
        => order.Id.ToString();
}

public class OrderIdAmountRetriever : IOrderDataRetriever
{
    public string Get(Order order)
        => $"{order.Id}{order.Amount}";
}

public class SecuredOrderRetriever : IOrderDataRetriever
{
    private readonly IOrderDataRetriever _orderDataRetriever;
    private readonly string _privateKey;
    
    public SecuredOrderRetriever(IOrderDataRetriever orderDataRetriever, string privateKey)
    {
        _orderDataRetriever = orderDataRetriever;
        _privateKey = privateKey;
    }
    
    public string Get(Order order)
    {
        return $"{_orderDataRetriever.Get(order)}{_privateKey}";
    }
}

public static class HashAlgorithmExtension
{
    public static string ComputeHash(this HashAlgorithm hasher, string input)
    {
        byte[] binaryData = Encoding.UTF8.GetBytes(input);
        byte[] hash = hasher.ComputeHash(binaryData);
        
        return Convert.ToHexString(hash);
    }
}

public abstract class PaymentQueryBuilder
{
    private readonly IOrderDataRetriever _orderDataRetriever;
    private readonly HashAlgorithm _hashAlgorithm;
    
    protected PaymentQueryBuilder(IOrderDataRetriever orderDataRetriever, HashAlgorithm hashAlgorithm)
    {
        _orderDataRetriever = orderDataRetriever;
        _hashAlgorithm = hashAlgorithm;
    }
    
    public abstract string Build(Order order, string currency);
    
    protected string CalculateOrderHash(Order order)
    {
        return _hashAlgorithm.ComputeHash(_orderDataRetriever.Get(order));
    }
}

public class PaymentQueryBuilder1(IOrderDataRetriever orderDataRetriever, HashAlgorithm hashAlgorithm)
    : PaymentQueryBuilder(orderDataRetriever, hashAlgorithm)
{
    public override string Build(Order order, string currency)
    {
        return $"amount={order.Amount}{currency}&hash={CalculateOrderHash(order)}";
    }
}

public class PaymentQueryBuilder2(IOrderDataRetriever orderDataRetriever, HashAlgorithm hashAlgorithm)
    : PaymentQueryBuilder(orderDataRetriever, hashAlgorithm)
{
    public override string Build(Order order, string currency)
    {
        return $"hash={CalculateOrderHash(order)}";
    }
}

public class PaymentQueryBuilder3(IOrderDataRetriever orderDataRetriever, HashAlgorithm hashAlgorithm)
    : PaymentQueryBuilder(orderDataRetriever, hashAlgorithm)
{
    public override string Build(Order order, string currency)
    {
        return $"amount={order.Amount}&currency={currency}&hash={CalculateOrderHash(order)}";
    }
}

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public class PaymentSystem : IPaymentSystem
{
    private readonly PaymentQueryBuilder _queryBuilder;
    private readonly string _paymentGateway;
    private readonly string _currency;
    
    public PaymentSystem(string paymentGateway, string currency, PaymentQueryBuilder queryBuilder)
    {
        _paymentGateway = paymentGateway;
        _queryBuilder = queryBuilder;
        _currency = currency;
    }
    
    public string GetPayingLink(Order order)
    {
        return $"{_paymentGateway}?{_queryBuilder.Build(order, _currency)}";
    }
}
