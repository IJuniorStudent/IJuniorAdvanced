using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace MvxDatabase;

static class Program
{
    private static void Main(string[] args)
    {
        string appRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string databaseName = "db.sqlite";
        
        var hasher = new HashCalculator(SHA256.Create());
        var selector = new SqliteSelectModel($"{appRootPath}\\{databaseName}");
        var presenter = new ConsolePresenter(selector, hasher);
        
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
            throw new ArgumentException("Empty passport id");
        
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
    
    public ConsolePresenter(IDatabaseSelectModel selector, IHashCalculator hasher)
    {
        _processor = new ProcessDataModel(selector, hasher);
        _view = new ConsoleView();
    }
    
    public void DisplayCitizenInfo()
    {
        try
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
        catch (ArgumentException e)
        {
            _view.DisplayMessage($"Failed to display citizen: {e.Message}");
        }
    }
}

public interface IDatabaseSelectModel
{
    public DataTable SelectPassportData(Passport passport, IHashCalculator hasher);
}

public class SqliteSelectModel : IDatabaseSelectModel
{
    private SqliteConnection _connection;
    
    public SqliteSelectModel(string databaseFilename)
    {
        _connection = new SqliteConnection($"Data Source={databaseFilename}");
    }
    
    public DataTable SelectPassportData(Passport passport, IHashCalculator hasher)
    {
        DataTable table = new DataTable();

        string passportHash = hasher.Calculate(passport.Id);
        string query = $"SELECT * FROM passports WHERE num = '{passportHash}' limit 1;";
        
        // _connection.Open();
        // Где-то тут делаем выборку, но в этой библиотеке апи другой, поэтому вот вам смайлик :)
        // _connection.Close();

        return table;
    }
}

public class ProcessDataModel
{
    private IDatabaseSelectModel _selector;
    private IHashCalculator _hasher;
    
    public ProcessDataModel(IDatabaseSelectModel selector, IHashCalculator hasher)
    {
        _selector = selector;
        _hasher = hasher;
    }
    
    public bool TryGetCitizen(Passport passport, out Citizen? citizen)
    {
        citizen = null;
        
        DataTable data = _selector.SelectPassportData(passport, _hasher);
        
        if (data.Rows.Count == 0)
            return false;
        
        citizen = new Citizen(passport, Convert.ToBoolean(data.Rows[0].ItemArray[1]));
        return true;
    }
}

public interface IHashCalculator
{
    public string Calculate(string input);
}

public class HashCalculator : IHashCalculator
{
    private HashAlgorithm _algorithm;
    
    public HashCalculator(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }
    
    public string Calculate(string input)
    {
        byte[] binaryInput = Encoding.UTF8.GetBytes(input);
        byte[] hash = _algorithm.ComputeHash(binaryInput);
        
        return Convert.ToHexString(hash);
    }
}
