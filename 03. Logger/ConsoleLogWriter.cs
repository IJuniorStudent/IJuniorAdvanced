namespace Logger;

public class ConsoleLogWriter : ILogWriter
{
    public void WriteError(string message)
    {
        Console.WriteLine(message);
    }
}