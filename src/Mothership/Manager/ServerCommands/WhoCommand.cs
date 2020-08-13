namespace Mothership.Manager.ServerCommands {
    public class WhoCommand: IServerCommand {
        public string Name {  get { return "who";  } }
        public string Syntax { get { return "who"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, params string[] args) {
            session.Client.WriteLine("Operators logged in:");
            session.Client.WriteLine("##############################################");
            session.Client.WriteLine("# ID               AccessType             IP #");
            session.Client.WriteLine("##############################################");
            foreach (var entry in server.Sessions)
                session.Client.WriteLine("# {0}  {1}  {2}  #", entry.Key, entry.Value.UserLevel, session.Client.IpAddress);
            session.Client.WriteLine("##############################################");
        }
    }
}
