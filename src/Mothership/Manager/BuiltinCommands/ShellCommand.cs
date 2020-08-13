using Mothership.Lp;

namespace Mothership.Manager.BuiltinCommands {
    public class ShellCommand: IBuiltinCommand {
        public string Name { get { return "shell"; } }
        public string Syntax { get { return "shell"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, MothershipConnection target, params string[] args) {
            session.UserLevel = TelnetUserLevel.Interactive;
        }
    }
}
