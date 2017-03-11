using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class ExitCommand : IClientCommand
    {
        public string Name { get { return "exit"; } }
        public string Syntax { get { return "exit"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 0);

            session.ChangeAccessLevel(AccessLevel.Server);
        }
    }
}
