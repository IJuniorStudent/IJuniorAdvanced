namespace Logger;

public class FileLogWriter : ILogWriter
{
    public void WriteError(string message)
    {
        File.AppendAllText("log.txt", $"{message}\n");
    }
}
