using System.Net.Sockets;

namespace MultiInput.Inputter
{
    public interface IInputReader
    {
        public void UpdateBindings(List<Socket> connections);
        public void OnCloseProgram();
    }
}
