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
        var logger = new ConsoleLogger();
        var hasher = new HashCalculator(SHA256.Create());
        
        var userDataValidator = new UserIdentifierValidator();
        var userDataGenerator = new UserIdentifierGenerator();
        var userDataRequester = new ConsoleUserDataRequester(userDataGenerator, userDataValidator);
        
        IIdentifier userIdentifier = userDataRequester.Get();
        var extractor = new UserIdentifierExtractor(userIdentifier);
        
        string appRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string databaseName = "db.sqlite";
        using var link = new Sqlite3Link($"{appRootPath}\\{databaseName}");
        
        if (link.TryConnect() == false)
        {
            logger.DisplayMessage($"Failed to connect database. Error: {link.ErrorMessage}");
            return;
        }
        
        var selector = new SqliteIdentifierDataSelector(extractor, hasher);
        DataTable selected = selector.Select(link);
        
        var checker = new VoteParticipantChecker(logger);
        checker.DisplayCheckResult(selected);
    }
}

public interface IDataTableChecker
{
    public void DisplayCheckResult(DataTable table);
}

public class VoteParticipantChecker : IDataTableChecker
{
    private ILogger _logger;
    
    public VoteParticipantChecker(ILogger logger)
    {
        _logger = logger;
    }
    
    public void DisplayCheckResult(DataTable table)
    {
        if (table.Rows.Count == 0)
        {
            _logger.DisplayMessage("По данному паспорту данные не найдены");
            return;
        }
        
        bool hasAccess = Convert.ToBoolean(table.Rows[0].ItemArray[1]);
        string accessResult = hasAccess ? "ПРЕДОСТАВЛЕН" : "НЕ ПРЕДОСТАВЛЯЛСЯ";
        
        _logger.DisplayMessage($"По данному паспорту доступ к бюллетеню на дистанционном электронном голосовании {accessResult}");
    }
}

public interface ILogger
{
    public void DisplayMessage(string message);
}

public class ConsoleLogger : ILogger
{
    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}

public interface IQueryTransformer
{
    public const int DefaultLimit = 0;
    public string GenerateQuery(string table, string condition, int limit = DefaultLimit);
}

public class SqliteQueryTransformer : IQueryTransformer
{
    public string GenerateQuery(string table, string condition, int limit)
    {
        string limitString = limit == IQueryTransformer.DefaultLimit ? "" : $" limit {limit}";
        return $"SELECT * FROM {table} WHERE {condition}{limitString};";
    }
}

public interface IDataSelector
{
    public DataTable Select(DatabaseLink link);
}

public struct SelectionPrefs
{
    public string Table { get; }
    public string Condition { get; }
    public int Limit { get; }
    
    public SelectionPrefs(string table, string condition, int limit)
    {
        Table = table;
        Condition = condition;
        Limit = limit;
    }
}

public abstract class IdentifierDataSelector : IDataSelector
{
    private IDataExtractor _extractor;
    private IHashCalculator _hasher;
    private IQueryTransformer _transformer;
    
    protected IdentifierDataSelector(IDataExtractor dataExtractor, IHashCalculator hasher, IQueryTransformer transformer)
    {
        _extractor = dataExtractor;
        _hasher = hasher;
        _transformer = transformer;
    }
    
    public DataTable Select(DatabaseLink link)
    {
        SelectionPrefs prefs = InitPrefs();
        string query = _transformer.GenerateQuery(prefs.Table, prefs.Condition, prefs.Limit);
        
        return link.Select(query);
    }
    
    protected string CalculateDataHash()
    {
        return _hasher.Calculate(_extractor.Data);
    }
    
    protected abstract SelectionPrefs InitPrefs();
}

public class SqliteIdentifierDataSelector : IdentifierDataSelector
{
    public SqliteIdentifierDataSelector(IDataExtractor dataExtractor, IHashCalculator hasher) : base(dataExtractor, hasher, new SqliteQueryTransformer()) { }
    
    protected override SelectionPrefs InitPrefs()
    {
        return new SelectionPrefs("passports", $"num = '{CalculateDataHash()}'", 1);
    }
}

public abstract class DatabaseLink : IDisposable
{
    private readonly string _connectionAddress;
    
    protected DatabaseLink(string address)
    {
        _connectionAddress = address;
        ErrorMessage = "";
    }
    
    public string ErrorMessage { get; protected set; }
    
    public bool TryConnect()
    {
        return TryInitializeConnection(_connectionAddress);
    }
    
    public void Dispose()
    {
        Disconnect();
    }
    
    public abstract DataTable Select(string query);
    
    protected abstract bool TryInitializeConnection(string address);
    protected abstract void Disconnect();
}

public class Sqlite3Link : DatabaseLink
{
    private SqliteConnection? _connection = null;
    
    public Sqlite3Link(string address) : base(address) { }
    
    public override DataTable Select(string query)
    {
        throw new NotImplementedException();
    }

    protected override bool TryInitializeConnection(string address)
    {
        if (File.Exists(address) == false)
        {
            ErrorMessage = $"Database file \"{address}\" does not exist";
            return false;
        }
        
        try
        {
            _connection = new SqliteConnection($"Data Source={address}");
            _connection.Open();
            return true;
        }
        catch (SqliteException e)
        {
            ErrorMessage = e.Message;
            return false;
        }
    }
    
    protected override void Disconnect()
    {
        _connection?.Close();
        _connection = null;
    }
}

public interface IUserDataRequester
{
    public IIdentifier Get();
}

public abstract class UserDataRequester : IUserDataRequester
{
    private readonly IIdentifierGenerator _generator;
    private readonly IIdentifierValidator _validator;
    
    protected UserDataRequester(IIdentifierGenerator generator, IIdentifierValidator validator)
    {
        _generator = generator;
        _validator = validator;
    }
    
    public IIdentifier Get()
    {
        string userData = RetrieveUserData();
        return _generator.Create(userData, _validator);
    }

    protected abstract string RetrieveUserData();
}

public class ConsoleUserDataRequester : UserDataRequester
{
    public ConsoleUserDataRequester(IIdentifierGenerator generator, IIdentifierValidator validator) : base(generator, validator) {}
    
    protected override string RetrieveUserData()
    {
        return Console.ReadLine()!;
    }
}

public interface IHashCalculator
{
    public string Calculate(string input);
}

public class HashCalculator : IHashCalculator
{
    private HashAlgorithm _algorithm;
    
    public HashCalculator(HashAlgorithm hashAlgorithm)
    {
        _algorithm = hashAlgorithm;
    }
    
    public string Calculate(string input)
    {
        byte[] binaryData = Encoding.UTF8.GetBytes(input);
        byte[] binaryHash = _algorithm.ComputeHash(binaryData);
        
        return Convert.ToHexString(binaryHash);
    }
}

public interface IIdentifier
{
    public string Id { get; }
    public string DisplayId { get; }
}

public class UserIdentifier : IIdentifier
{
    public UserIdentifier(string identifier, IIdentifierValidator validator)
    {
        validator.Validate(identifier);
        
        Id = validator.Id;
        DisplayId = validator.DisplayId;
    }
    
    public string Id { get; }
    public string DisplayId { get; }
}

public interface IIdentifierValidator : IIdentifier
{
    public void Validate(string input);
}

public class UserIdentifierValidator : IIdentifierValidator
{
    private const int MinIdLength = 10;
    private const string WhiteSpace = " ";
    private const string EmptyString = "";

    private string _id = "";
    private string _displayId = "";
    
    public void Validate(string input)
    {
        _displayId = input.Trim();
        
        if (_displayId == EmptyString)
            throw new ArgumentException("Identifier is empty");

        _id = _displayId.Replace(WhiteSpace, EmptyString);
        
        if (_id.Length < MinIdLength)
            throw new ArgumentException($"Identifier must contain at least {MinIdLength} characters");
    }
    
    public string Id => _id;
    public string DisplayId => _displayId;
}

public interface IDataExtractor
{
    public string Data { get; }
}

public class UserIdentifierExtractor : IDataExtractor
{
    private readonly IIdentifier _identifier;
    
    public UserIdentifierExtractor(IIdentifier identifier)
    {
        _identifier = identifier;
    }
    
    public string Data => _identifier.Id;
}

public interface IIdentifierGenerator
{
    public IIdentifier Create(string id, IIdentifierValidator validator);
}

public class UserIdentifierGenerator : IIdentifierGenerator
{
    public IIdentifier Create(string id, IIdentifierValidator validator)
    {
        return new UserIdentifier(id, validator);
    }
}
