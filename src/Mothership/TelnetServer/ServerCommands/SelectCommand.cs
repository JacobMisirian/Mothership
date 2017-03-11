using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class SelectCommand : IServerCommand
    {
        public string Name {  get { return "select"; } }
        public string Syntax {  get { return "select [CLIENT]"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 1);

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

            session.SelectClient(selectedClient);
        }
    }
}
