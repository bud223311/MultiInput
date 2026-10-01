using System.Net.Sockets;
using MultiInput.Configuration;

namespace MultiInput.Inputter
{
    public interface IInputReader
    {
        public void UpdateBindings(List<Socket> connections,ConfigurationFile config);
        public void OnCloseProgram();
    }
}
