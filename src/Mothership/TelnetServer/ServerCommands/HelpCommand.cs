using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class HelpCommand : IServerCommand
    {
        public string Name { get { return "help"; } }
        public string Syntax { get { return "help"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            user.WriteLine("Available Server Commands");
            user.WriteLine("############################");
            foreach (var command in server.ServerCommands.Values)
                user.WriteLineCentered("#", command.Syntax, "#", 28);
            user.WriteLine("############################");
        }
    }
}
