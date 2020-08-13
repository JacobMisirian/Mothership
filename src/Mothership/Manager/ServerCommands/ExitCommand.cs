namespace Mothership.Manager.ServerCommands {
    public class ExitCommand : IServerCommand {
        public string Name { get { return "exit"; } }
        public string Syntax { get { return "exit"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, params string[] args) {
            server.EndSession(session.Client.Id);
        }
    }
}
