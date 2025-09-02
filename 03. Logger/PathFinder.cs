namespace Logger;

public class PathFinder
{
    private ILogWriter _logWriter;
    
    public PathFinder(ILogWriter logWriter)
    {
        _logWriter = logWriter;
    }
    
    public void Find()
    {
        _logWriter.WriteError("Nothing found");
    }
}