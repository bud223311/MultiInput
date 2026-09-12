namespace MultiInput.Poller;

public static class TcpPoll
{
    public static bool? TimeUntilPollAfterInput(int i){
        if (StaticData.TimesinceLastDataSent is null) {
            return null;
        }
        if (DateTime.Now > StaticData.TimesinceLastDataSent.Value.AddSeconds(i)) {
            StaticData.TimesinceLastDataSent = DateTime.Now;
            Console.WriteLine($"Sending ping to check connection");
            return true;
        }
        return false;
    }
}