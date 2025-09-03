namespace Logger;

public class WeeklyLogWriter : ILogWriter
{
    private readonly ILogWriter _logWriter;
    private readonly DayOfWeek _logWriteDay;
    
    public WeeklyLogWriter(ILogWriter logWriter, DayOfWeek dayOfWeek)
    {
        _logWriter = logWriter;
        _logWriteDay = dayOfWeek;
    }
    
    public void WriteError(string message)
    {
        if (DateTime.Now.DayOfWeek == _logWriteDay)
            _logWriter.WriteError(message);
    }
}