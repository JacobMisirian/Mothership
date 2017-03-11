using Mothership.Networking;

namespace Mothership.TelnetServer
{
    public interface IClientCommand
    {
        string Name { get; }
        string Syntax { get; }
        void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args);
    }
}
