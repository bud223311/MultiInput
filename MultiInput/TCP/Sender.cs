using System.Net.Sockets;
using System.Text;

namespace MultiInput.TCP;

internal class TcpSender
{
    internal static void ServerSender(string ip, int port)
    {
        FileStream fs = new(TcpServer.LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 8);
        {
            int FailCount = 0;

            var newlineString = Environment.NewLine;
            byte[] newline = Encoding.UTF8.GetBytes(newlineString);

            fs.Position = fs.Seek(0, SeekOrigin.End);
            fs.Write(newline, 0, newlineString.Length);
            //debug

            Console.WriteLine("Ready to send data.");
            while (true)
            {
                //add read log path stream for getting data from ahk
                //add wait for client connection (30s?)
                try
                {
                    int key = fs.ReadByte();

                    TcpClient client = new(ip, port);
                    NetworkStream stream = client.GetStream();

                    Thread.Sleep(3000);

                    byte[] data = Encoding.UTF8.GetBytes(key.ToString());
                    stream.Write(data, 0, data.Length);

                    client.Close();
                }
                catch
                {
                    FailCount++;
                    Console.WriteLine($"Unable to connect to client, retrying.. (Attempt {FailCount})");
                    Thread.Sleep(3000);
                }
            }
        }
    }
}