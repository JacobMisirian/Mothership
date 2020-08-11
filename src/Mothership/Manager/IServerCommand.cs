namespace Mothership.Manager {
    public interface IServerCommand {
        string Name { get; }
        string Syntax { get; }
        void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, params string[] args);
    }
}
