namespace Polymorph;

class Program
{
    static void Main(string[] args)
    {
        var paymentSystems = new PaymentSystemFactoryStorage();
        var consoleView = new ConsolePaymentView();
        var paymentForm = new PaymentForm(consoleView);
        
        if (paymentForm.TryGetPaymentMethod(paymentSystems.Options, out string paymentMethod) == false)
        {
            consoleView.DisplayMessage($"Система \"{paymentMethod}\" не найдена");
            return;
        }
        
        var handler = new PaymentHandler(paymentSystems.GetFactory(paymentMethod));
        
        handler.Pay(consoleView);
    }
}

public interface IPaymentForm
{
    public bool TryGetPaymentMethod(List<string> methodNames, out string paymentMethod);
}

public class PaymentForm : IPaymentForm
{
    private IPaymentView _view;
    
    public PaymentForm(IPaymentView view)
    {
        _view = view;
    }
    
    public bool TryGetPaymentMethod(List<string> methodNames, out string paymentMethod)
    {
        string availableMethods = string.Join(", ", methodNames);
        
        _view.DisplayMessage($"Мы принимаем: {availableMethods}");
        _view.DisplayMessage("Какое системой вы хотите совершить оплату?");
        
        paymentMethod = _view.RequestUserInput();
        
        return methodNames.Contains(paymentMethod);
    }
}

public class PaymentHandler
{
    private IPaymentSystem _system;
    
    public PaymentHandler(PaymentSystemFactory factory)
    {
        _system = factory.Create();
    }
    
    public void Pay(IPaymentView paymentView)
    {
        _system.Initialize(paymentView);
        paymentView.DisplayMessage(_system.Pay(paymentView) ? "Оплата прошла успешно!" : "Что-то пошло не так");
    }
}

public interface IPaymentView
{
    void DisplayMessage(string message);
    string RequestUserInput();
}

public class ConsolePaymentView : IPaymentView
{
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
    
    public string RequestUserInput()
    {
        return Console.ReadLine() ?? "";
    }
}

public interface IPaymentSystem
{
    public string Name { get; }
    public void Initialize(IPaymentView paymentView);
    public bool Pay(IPaymentView paymentView);
}

public class PaymentSystem : IPaymentSystem
{
    private string _requestMessage;
    
    public PaymentSystem(string name, string requestMessage)
    {
        Name = name;
        _requestMessage = requestMessage;
    }
    
    public string Name { get; }
    
    public void Initialize(IPaymentView paymentView)
    {
        paymentView.DisplayMessage($"{_requestMessage} {Name}...");
    }
    
    public bool Pay(IPaymentView paymentView)
    {
        paymentView.DisplayMessage($"Вы оплатили с помощью {Name}");
        return Verify(paymentView);
    }
    
    private bool Verify(IPaymentView paymentView)
    {
        paymentView.DisplayMessage($"Проверка платежа через {Name}");
        return true;
    }
}

public class PaymentSystemFactory
{
    private string _name;
    private string _requestMessage;
    
    public PaymentSystemFactory(string name, string requestMessage)
    {
        _name = name;
        _requestMessage = requestMessage;
    }
    
    public IPaymentSystem Create()
    {
        return new PaymentSystem(_name, _requestMessage);
    }
}

public class PaymentSystemFactoryStorage
{
    private Dictionary<string, PaymentSystemFactory> _systems;
    
    public PaymentSystemFactoryStorage()
    {
        _systems = new Dictionary<string, PaymentSystemFactory>();
        
        AppendPaymentSystemFactory("QIWI", "Перевод на страницу");
        AppendPaymentSystemFactory("WebMoney", "Вызов API");
        AppendPaymentSystemFactory("Card", "Вызов API банка эмиттера карты");
    }
    
    public List<string> Options => _systems.Keys.ToList();
    
    public PaymentSystemFactory GetFactory(string name)
    {
        return _systems[name];
    }
    
    private void AppendPaymentSystemFactory(string systemName, string systemRequestMessage)
    {
        _systems.Add(systemName, new PaymentSystemFactory(systemName, systemRequestMessage));
    }
}
