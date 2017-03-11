using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class InfoCommand : IClientCommand
    {
        public string Name { get { return "info"; } }
        public string Syntax { get { return "info"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            user.WriteLine("Client '{0}' Banner", session.SelectedClient);
            user.WriteLine("#######################################################");
            foreach (string line in server.ClientServer.Clients[session.SelectedClient].Banner.Split('\n'))
                user.WriteLineCentered("#", line.Replace("\r", ""), "#", 55);
            user.WriteLine("#######################################################");
        }
    }
}
