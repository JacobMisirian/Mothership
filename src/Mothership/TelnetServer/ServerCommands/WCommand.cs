using Mothership.Networking;

namespace Mothership.TelnetServer.ServerCommands
{
    public class WCommand : IServerCommand
    {
        public string Name { get { return "w"; } }
        public string Syntax { get { return "w"; } }

        public void Invoke(TelnetServer server, TcpClient user, TelnetSession session, params string[] args)
        {
            user.WriteLine("Users Logged In");
            user.WriteLine("##############################################");
            user.WriteLine("ID    AccessType     SelectedClient         IP");
            user.WriteLine("##############################################");
            foreach (var entry in server.Sessions)
                user.WriteLineCentered("#", string.Format("{0}  {1}  {2}  {3}", entry.Key, entry.Value.AccessLevel, entry.Value.SelectedClient, server.Users[session.UID].IP), "#", 46);
            user.WriteLine("##############################################");
        }
    }
}
