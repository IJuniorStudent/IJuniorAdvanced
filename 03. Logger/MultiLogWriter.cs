namespace Logger;

public class MultiLogWriter : ILogWriter
{
    private ILogWriter[] _logWriters;
    
    public MultiLogWriter(params ILogWriter[] logWriters)
    {
        if (logWriters == null || logWriters.Length == 0)
            throw new ArgumentException("LogWriters must have at least one logWriter");
        
        for (int i = 0; i < logWriters.Length; i++)
            if (logWriters[i] == null)
                throw new ArgumentException($"LogWriter at index {i} is null");
        
        _logWriters = logWriters;
    }
    
    public void WriteError(string message)
    {
        foreach (var logWriter in _logWriters)
            logWriter.WriteError(message);
    }
}