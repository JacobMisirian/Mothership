namespace Mothership.Manager.ServerCommands {
    public class SelectCommand : IServerCommand {
        public string Name { get { return "select"; } }
        public string Syntax {  get { return "select [client_id]"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, params string[] args) {
            string clientId = args[0];

            if (!server.Lp.Connections.ContainsKey(clientId)) {
                session.Client.WriteLine("No such client connected with ID \"{0}\"!", clientId);
                return;
            }

            session.SelectedClient = server.Lp.Connections[clientId];
            session.UserLevel = TelnetUserLevel.Client;
        }
    }
}
