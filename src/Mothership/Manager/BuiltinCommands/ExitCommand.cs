using Mothership.Lp;

namespace Mothership.Manager.BuiltinCommands {
    public class ExitCommand : IBuiltinCommand {
        public string Name { get { return "exit"; } }
        public string Syntax { get { return "exit"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, MothershipConnection target, params string[] args) {
            session.SelectedClient = null;
            session.UserLevel = TelnetUserLevel.Server;
        }
    }
}
