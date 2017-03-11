using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class ShowCommand : IServerCommand
    {
        public string Name {  get { return "show"; } }
        public string Syntax {  get { return "show"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 0);

            user.WriteLine("Established Connections");
            user.WriteLine("########################");
            foreach (var client in server.ClientServer.Clients.Keys)
                user.WriteLineCentered("#", client , "#", 24);
            user.WriteLine("########################");
        }
    }
}
