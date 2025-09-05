using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace MvxDatabase;

static class Program
{
    private static void Main(string[] args)
    {
        string appRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string databaseName = "db.sqlite";
        
        var selector = new SqliteSelectModel($"{appRootPath}\\{databaseName}");
        var presenter = new ConsolePresenter(selector);
        
        presenter.DisplayCitizenInfo();
    }
}

public class Passport
{
    private const int MinIdLength = 10;
    private const string WhiteSpace = " ";
    private const string EmptyString = "";
    
    public Passport(string id)
    {
        string passportId = id.Trim();
        
        if (passportId == EmptyString)
            throw new ArgumentException("Empty id");
        
        Id = passportId.Replace(WhiteSpace, EmptyString);
        
        if (Id.Length < MinIdLength)
            throw new ArgumentException($"Passport id must contain at least {MinIdLength} characters");
    }
    
    public string Id { get; }
}

public class Citizen
{
    public Citizen(Passport passport, bool hasVoteAccess)
    {
        Passport = passport;
        HasVoteAccess = hasVoteAccess;
    }
    
    public Passport Passport { get; }
    public bool HasVoteAccess { get; }
}

public interface IView
{
    public void DisplayMessage(string message);
    public string RequestPassportId();
}

public class ConsoleView : IView
{
    public void DisplayMessage(string message)
    {
        Console.WriteLine();
    }
    
    public string RequestPassportId()
    {
        return Console.ReadLine()!;
    }
}

public interface IPresenter
{
    public void DisplayCitizenInfo();
}

public class ConsolePresenter : IPresenter
{
    private readonly ProcessDataModel _processor;
    private readonly IView _view;
    
    public ConsolePresenter(IDatabaseSelectModel selector)
    {
        _processor = new ProcessDataModel(selector);
        _view = new ConsoleView();
    }
    
    public void DisplayCitizenInfo()
    {
        var passport = new Passport(_view.RequestPassportId());
        
        if (_processor.TryGetCitizen(passport, out Citizen? citizen) == false)
        {
            _view.DisplayMessage($"Данные по паспорту \"{passport.Id}\" не найдены");
            return;
        }
        
        string accessResult = citizen!.HasVoteAccess ? "ПРЕДОСТАВЛЕН" : "НЕ ПРЕДОСТАВЛЯЛСЯ";
        
        _view.DisplayMessage($"По паспорту \"{citizen.Passport.Id}\" доступ к бюллетеню на дистанционном электронном голосовании {accessResult}");
    }
}

public interface IDatabaseSelectModel
{
    public DataTable SelectPassportData(Passport passport);
}

public class SqliteSelectModel : IDatabaseSelectModel
{
    private SqliteConnection _connection;
    
    public SqliteSelectModel(string databaseFilename)
    {
        _connection = new SqliteConnection($"Data Source={databaseFilename}");
    }
    
    public DataTable SelectPassportData(Passport passport)
    {
        DataTable table = new DataTable();
        
        // _connection.Open();
        // Где-то тут делаем выборку, но в этой библиотеке апи другой, поэтому вот вам смайлик вместо исключения - :)
        // _connection.Close();

        return table;
    }
}

public class ProcessDataModel
{
    private IDatabaseSelectModel _selector;
    
    public ProcessDataModel(IDatabaseSelectModel selector)
    {
        _selector = selector;
    }
    
    public bool TryGetCitizen(Passport passport, out Citizen? citizen)
    {
        citizen = null;
        
        DataTable data = _selector.SelectPassportData(passport);
        
        if (data.Rows.Count == 0)
            return false;
        
        citizen = new Citizen(passport, Convert.ToBoolean(data.Rows[0].ItemArray[1]));
        return true;
    }
}
