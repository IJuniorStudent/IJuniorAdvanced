namespace Logger;

class Program
{
    static void Main(string[] args)
    {
        ILogWriter fileLogWriter = new FileLogWriter();
        ILogWriter consoleLogWriter = new ConsoleLogWriter();
        ILogWriter fridayFileLogWriter = new FridayLogWriter(fileLogWriter);
        ILogWriter fridayConsoleLogWriter = new FridayLogWriter(consoleLogWriter);
        ILogWriter multiLogWriter = new MultiLogWriter(consoleLogWriter, fridayFileLogWriter);
        
        PathFinder[] pathFinders =
        [
            new (fileLogWriter),
            new (consoleLogWriter),
            new (fridayFileLogWriter),
            new (fridayConsoleLogWriter),
            new (multiLogWriter)
        ];
        
        foreach (var pathFinder in pathFinders)
            pathFinder.Find();
    }
}