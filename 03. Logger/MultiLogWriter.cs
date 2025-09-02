namespace Logger;

public class MultiLogWriter : ILogWriter
{
    private ILogWriter[] _logWriters;
    
    public MultiLogWriter(params ILogWriter[] logWriters)
    {
        if (logWriters.Length == 0)
            throw new ArgumentException("LogWriters must have at least one logWriter");
        
        _logWriters = logWriters;
    }
    
    public void WriteError(string message)
    {
        foreach (var logWriter in _logWriters)
            logWriter.WriteError(message);
    }
}