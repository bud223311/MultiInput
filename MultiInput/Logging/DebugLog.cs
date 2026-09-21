namespace MultiInput.Logging;

public static class DebugLog
{
    private static StreamWriter? _debugWriter;
    private static void CreateIfNotExists(){
        if (!Directory.Exists($"{Environment.CurrentDirectory}/Debug")) {
            Directory.CreateDirectory($"{Environment.CurrentDirectory}/Debug");
        }
        if (!File.Exists($"{Environment.CurrentDirectory}/Debug/DebugLog.txt")) {
            File.Create($"{Environment.CurrentDirectory}/Debug/DebugLog.txt").Close();
        }
    }

    public static void ClearDebugLog(){
        if (!File.Exists($"{Environment.CurrentDirectory}/Debug/DebugLog.txt")) {
            ConsoleLog.WriteConsoleMessage($"No Debug Log Found to Clear", ConsoleColor.Yellow);
            return;
        }
        _debugWriter?.Flush();
        _debugWriter?.Close();
        File.WriteAllText($"{Environment.CurrentDirectory}/Debug/DebugLog.txt", string.Empty);
    }
    public static void CreatePreviousDebugLog(){
        if (!File.Exists($"{Environment.CurrentDirectory}/Debug/DebugLog.txt")) {
            ConsoleLog.WriteConsoleMessage($"No Debug Log Found to Create Previous Log", ConsoleColor.Yellow);
            return;
        }
        string previousLogPath = $"{Environment.CurrentDirectory}/Debug/DebugLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        File.Copy($"{Environment.CurrentDirectory}/Debug/DebugLog.txt", previousLogPath);
        ClearDebugLog();
    }
    private static void OpenSteamWriter(){
        CreateIfNotExists();
        _debugWriter = new StreamWriter($"{Environment.CurrentDirectory}/Debug/DebugLog.txt", true);
        _debugWriter.WriteLine($"[{DateTime.Now}] Debug Log Opened");
    }

    public static void InitializeStartup(){
        OpenSteamWriter();
    }
    public static string GetPathOfDebugLog(){
        return $"{Environment.CurrentDirectory}/Debug/DebugLog.txt";
    }

    public static void DebugMessageThread(string message, bool includeTimeStamp = true){
        new Thread(() => WriteDebugMessage(message, includeTimeStamp)).Start();
    }
    public static void WriteDebugMessage(string message, bool includeTimeStamp = true){
        
        if (_debugWriter is null) {
            OpenSteamWriter();
        }
        _debugWriter?.WriteLine($"[{(includeTimeStamp ? DateTime.Now : string.Empty)}] {message}");
        _debugWriter?.Flush();
    }
    
    public static void CloseDebugLog(){
        _debugWriter?.WriteLine($"[{DateTime.Now}] Debug Log Closed");
        _debugWriter?.Flush();
        _debugWriter?.Close();
    }
}