using Mothership.Lp;

namespace Mothership.Manager {
    public interface IClientMacro {
        string Name { get; }
        string Syntax { get; }
        void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, MothershipConnection target, params string[] args);
    }
}
