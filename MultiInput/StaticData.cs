using MultiInput.TCP;

namespace MultiInput;

public static class StaticData
{
    public static DateTime StartTime => DateTime.Now;
    public static DateTime? TimesinceLastDataSent;
    public static bool WasDisconnected = false;
    internal static bool ToggleInput = true;
    public static MultiInputTcp.KTcpHost? Host;
    public static MultiInputTcp.KTcpClient? Client;
}