namespace Logger;

class Program
{
    static void Main(string[] args)
    {
        const DayOfWeek logWriteDay = DayOfWeek.Friday;
        
        ILogWriter fileLogWriter = new FileLogWriter();
        ILogWriter consoleLogWriter = new ConsoleLogWriter();
        ILogWriter weeklyFileLogWriter = new WeeklyLogWriter(fileLogWriter, logWriteDay);
        ILogWriter weeklyConsoleLogWriter = new WeeklyLogWriter(consoleLogWriter, logWriteDay);
        ILogWriter multiLogWriter = new MultiLogWriter(consoleLogWriter, weeklyFileLogWriter);
        
        PathFinder[] pathFinders =
        [
            new (fileLogWriter),
            new (consoleLogWriter),
            new (weeklyFileLogWriter),
            new (weeklyConsoleLogWriter),
            new (multiLogWriter)
        ];
        
        foreach (var pathFinder in pathFinders)
            pathFinder.Find();
    }
}