using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class HelpCommand : IClientCommand
    {
        public string Name { get { return "help"; } }
        public string Syntax { get { return "help"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            user.WriteLine("Available Client Commands");
            user.WriteLine("#########################");
            foreach (var command in server.ClientCommands.Values)
                user.WriteLineCentered("#", command.Syntax, "#", 25);
            user.WriteLine("#########################");
        }
    }
}
