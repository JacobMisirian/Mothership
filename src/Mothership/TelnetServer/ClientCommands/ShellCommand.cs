using Mothership.Networking;

namespace Mothership.TelnetServer.ClientCommands
{
    public class ShellCommand : IClientCommand
    {
        public string Name { get { return "shell"; } }
        public string Syntax {  get { return "shell"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 0);

            session.ChangeAccessLevel(AccessLevel.Shell);
        }
    }
}