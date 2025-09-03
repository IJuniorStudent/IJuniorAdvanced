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
        
        var order = new Order(orderId, orderAmount);
        
        PaymentSystem[] paymentSystems =
        [
            new PaymentSystem1(),
            new PaymentSystem2(),
            new PaymentSystem3(privateSignKey)
        ];
        
        foreach (var paymentSystem in paymentSystems)
            Console.WriteLine(paymentSystem.GetPayingLink(order));
    }
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;
    
    public Order(int id, int amount)
    {
        Id = id;
        Amount = amount;
    }
}

public interface IOrderDataRetriever
{
    public string Get(Order order);
}

public class OrderIdRetriever : IOrderDataRetriever
{
    public string Get(Order order) =>
        order.Id.ToString();
}

public class OrderIdAmountRetriever : IOrderDataRetriever
{
    public string Get(Order order) =>
        $"{order.Id}{order.Amount}";
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
    
    public string Get(Order order) =>
        $"{_orderDataRetriever.Get(order)}{_privateKey}";
}

public interface IHashCalculator
{
    public string Calculate(string input);
}

public class HashCalculator : IHashCalculator
{
    private readonly HashAlgorithm _hashAlgorithm;
    
    public HashCalculator(HashAlgorithm hashAlgorithm)
    {
        _hashAlgorithm = hashAlgorithm;
    }
    
    public string Calculate(string input)
    {
        byte[] binaryData = Encoding.UTF8.GetBytes(input);
        byte[] hash = _hashAlgorithm.ComputeHash(binaryData);
        
        return Convert.ToHexString(hash);
    }
}

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public abstract class PaymentSystem : IPaymentSystem
{
    private readonly string _paymentGateway;
    private readonly string _currency;
    private readonly IOrderDataRetriever _dataRetriever;
    private readonly IHashCalculator _hasher;
    
    protected PaymentSystem(string paymentGateway, string currency, IOrderDataRetriever dataRetriever, IHashCalculator hasher)
    {
        _paymentGateway = paymentGateway;
        _currency = currency;
        _dataRetriever = dataRetriever;
        _hasher = hasher;
    }
    
    public string GetPayingLink(Order order)
    {
        return $"{_paymentGateway}?{BuildQueryString(order, _currency)}";
    }
    
    protected abstract string BuildQueryString(Order order, string currency);
    
    protected string CalculateHash(Order order)
    {
        return _hasher.Calculate(_dataRetriever.Get(order));
    }
}

public class PaymentSystem1()
    : PaymentSystem("pay.system1.ru/order", "RUB", new OrderIdRetriever(), new HashCalculator(MD5.Create()))
{
    protected override string BuildQueryString(Order order, string currency)
    {
        return $"amount={order.Amount}{currency}&hash={CalculateHash(order)}";
    }
}

public class PaymentSystem2()
    : PaymentSystem("order.system2.ru/pay", "RUB", new OrderIdAmountRetriever(), new HashCalculator(MD5.Create()))
{
    protected override string BuildQueryString(Order order, string currency)
    {
        return $"hash={CalculateHash(order)}";
    }
}

public class PaymentSystem3(string privateSignKey)
    : PaymentSystem("system3.com/pay", "RUB", new SecuredOrderRetriever(new OrderIdAmountRetriever(), privateSignKey), new HashCalculator(SHA1.Create()))
{
    protected override string BuildQueryString(Order order, string currency)
    {
        return $"amount={order.Amount}&currency={currency}&hash={CalculateHash(order)}";
    }
}
