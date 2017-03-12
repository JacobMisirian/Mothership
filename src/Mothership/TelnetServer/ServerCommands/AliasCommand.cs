using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class AliasCommand : IServerCommand
    {
        public string Name { get { return "alias"; } }
        public string Syntax { get { return "alias [CLIENT] [NEW_NAME]"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 2);

            if (args[1] == "all")
            {
                user.WriteLine("Error! Cannot alias client to 'all', reserved word!");
                return;
            }

            string selectedClient = string.Empty;
            foreach (var client in server.ClientServer.Clients.Keys)
            {
                if (client.StartsWith(args[0]))
                {
                    selectedClient = client;
                    break;
                }
            }

            if (selectedClient == string.Empty)
            {
                user.WriteLine("Error! No such client {0}! Use 'show' to display a list of clients.", selectedClient);
                return;
            }

            var temp = server.ClientServer.Clients[selectedClient];
            temp.UID = args[1];
            server.ClientServer.Clients.Remove(selectedClient);
            server.ClientServer.Clients.Add(temp.UID, temp);
        }
    }
}
