namespace MultiInput.Logging;

public static class ConsoleLog
{
    public static void WriteConsoleMessage(string message, bool includeTimeStamp = true){
        Console.WriteLine($"[{(includeTimeStamp ? DateTime.Now : string.Empty)}] {message}");
    }
    public static void WriteConsoleMessage(string message, ConsoleColor color, bool includeTimeStamp = true){
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{(includeTimeStamp ? DateTime.Now : string.Empty)}] {message}");
        Console.ForegroundColor = originalColor;
    }
    public static void WriteOnlyColoredTimeStamp(string message, ConsoleColor color, bool includeTimeStamp = true){
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write($"[{(includeTimeStamp ? DateTime.Now : string.Empty)}]");
        Console.ForegroundColor = originalColor;
        Console.WriteLine($" {message}");
    }
}