namespace Polymorph;

class Program
{
    static void Main(string[] args)
    {
        var paymentSystemFactory = new PaymentSystemFactory();
        var consoleView = new ConsolePaymentView();
        var paymentForm = new PaymentForm(consoleView, paymentSystemFactory.Create());
        
        paymentForm.Show();
        paymentForm.Pay();
    }
}

public interface IPaymentForm
{
    void Show();
    void Pay();
}

public class PaymentForm : IPaymentForm
{
    private IPaymentView _view;
    private List<IPaymentSystem> _systems;
    
    public PaymentForm(IPaymentView view, List<IPaymentSystem> systems)
    {
        _view = view;
        _systems = systems;
    }
    
    public void Show()
    {
        _view.DisplayMessage($"Мы принимаем: {GetPaymentOptions()}");
        _view.DisplayMessage("Какое системой вы хотите совершить оплату?");
    }
    
    public void Pay()
    {
        string option = _view.RequestUserInput();
        
        if (TryGetPaymentSystem(option, out IPaymentSystem? paymentSystem) == false)
        {
            _view.DisplayMessage($"Система оплаты \"{option}\" не найдена");
            return;
        }
        
        paymentSystem!.Initialize(_view);
        
        _view.DisplayMessage(paymentSystem.Pay(_view) ? "Оплата прошла успешно!" : "Что-то пошло не так");
    }
    
    private string GetPaymentOptions()
    {
        var options = new List<string>();
        
        foreach (IPaymentSystem system in _systems)
            options.Add(system.Name);
        
        return string.Join(", ", options);
    }
    
    private bool TryGetPaymentSystem(string name, out IPaymentSystem? foundSystem)
    {
        foundSystem = null;
        
        foreach (IPaymentSystem system in _systems)
        {
            if (system.Name != name)
                continue;
            
            foundSystem = system;
            return true;
        }
        
        return false;
    }
}

public interface IPaymentView
{
    void DisplayHeader(string header);
    void DisplayMessage(string message);
    string RequestUserInput();
}

public class ConsolePaymentView : IPaymentView
{
    public void DisplayHeader(string header)
    {
        Console.WriteLine($"=== {header} ===");
    }

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

public abstract class PaymentSystem : IPaymentSystem
{
    private string _requestMessage;
    
    protected PaymentSystem(string name, string requestMessage)
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

public abstract class WebPaymentSystem : PaymentSystem
{
    protected WebPaymentSystem(string name) : base(name, "Перевод на страницу") { }
}

public abstract class ApiPaymentSystem : PaymentSystem
{
    protected ApiPaymentSystem(string name) : base(name, "Вызов API") { }
}

public abstract class BankPaymentSystem : PaymentSystem
{
    protected BankPaymentSystem(string name) : base(name, "Вызов API банка эмиттера карты") { }
}

public class QiwiPaymentSystem : WebPaymentSystem
{
    public QiwiPaymentSystem() : base("QIWI") { }
}

public class WebMoneyPaymentSystem : ApiPaymentSystem
{
    public WebMoneyPaymentSystem() : base("WebMoney") { }
}

public class CardPaymentSystem : BankPaymentSystem
{
    public CardPaymentSystem() : base("Card") { }
}

public class PaymentSystemFactory
{
    public List<IPaymentSystem> Create()
    {
        List<IPaymentSystem> systems =
        [
            new QiwiPaymentSystem(),
            new WebMoneyPaymentSystem(),
            new CardPaymentSystem()
        ];
        
        return systems;
    }
}
