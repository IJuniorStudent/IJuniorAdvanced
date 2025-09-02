namespace Logger;

public class FridayLogWriter : ILogWriter
{
    private readonly ILogWriter _logWriter;
    
    public FridayLogWriter(ILogWriter logWriter)
    {
        _logWriter = logWriter;
    }
    
    public void WriteError(string message)
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            _logWriter.WriteError(message);
    }
}