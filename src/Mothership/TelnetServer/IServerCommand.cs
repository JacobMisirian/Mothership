using Mothership.Networking;

namespace Mothership.TelnetServer
{
    public interface IServerCommand
    {
        string Name { get; }
        string Syntax { get; }
        void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args);
    }
}
