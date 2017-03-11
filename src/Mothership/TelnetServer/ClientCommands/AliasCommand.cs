using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class AliasCommand : IClientCommand
    {
        public string Name { get { return "alias"; } }
        public string Syntax { get { return "alias [NEW_NAME]"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 1);

            var temp = server.ClientServer.Clients[session.SelectedClient];
            temp.UID = args[0];
            server.ClientServer.Clients.Remove(session.SelectedClient);
            server.ClientServer.Clients.Add(temp.UID, temp);

            session.SelectClient(temp.UID);
        }
    }
}
