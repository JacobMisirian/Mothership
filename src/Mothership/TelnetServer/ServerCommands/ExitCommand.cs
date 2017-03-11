using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class ExitCommand : IServerCommand
    {
        public string Name {  get { return "exit"; } }
        public string Syntax {  get { return "exit"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 0);

            user.WriteLine("Goodbye!");
            user.BaseClient.Close();
        }
    }
}
