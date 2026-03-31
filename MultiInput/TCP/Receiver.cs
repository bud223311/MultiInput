using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MultiInput.TCP;

internal class TcpReceiver
{
    internal static void ServerListener(int port)
    {
        TcpListener server = new(IPAddress.Any, port);

        FileStream fs = new(TcpServer.LogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferSize: 8);
        {
            //Get newline byte value for formatting log
            var newlineString = Environment.NewLine;
            byte[] newline = Encoding.UTF8.GetBytes(newlineString);

            //Position file pointer to new line at file end
            fs.Position = fs.Seek(0, SeekOrigin.End);
            fs.WriteAsync(newline, 0, newlineString.Length);

            server.Start();
            Console.WriteLine($"Waiting for connection on port: {port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                byte[] message = new byte[data.Length];
                for (int i = 0; i < message.Length; i++)
                {
                    message[i] = buffer[i];
                }
                Console.WriteLine($"Received: {data}");

                //write newline before each new log entry
                fs.Write(newline, 0, newlineString.Length);
                fs.Write(message, 0, message.Length);

                Console.WriteLine($"{buffer[0]}");
                client.Close();
            }
        }
    }
}