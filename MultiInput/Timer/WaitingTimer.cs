namespace MultiInput.Timer;

public static class WaitingTimer
{
    public static void WaitForMilliseconds(double miliseconds){
        DateTime waitTime = DateTime.Now.AddMilliseconds(miliseconds);
        while (DateTime.Now < waitTime);
    }
}