using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class DisconnectCommand : IClientCommand
    {
        public string Name { get { return "disconnect"; } }
        public string Syntax { get { return "disconnect"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            server.DisconnectClient(server.ClientServer.Clients[session.SelectedClient]);
        }
    }
}