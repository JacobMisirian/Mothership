using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.Manager.ServerCommands {
    public class ListCommand : IServerCommand {
        public string Name { get { return "list"; } }
        public string Syntax { get { return "list"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, params string[] args) {
            if (server.Lp.Connections.Count == 0)
                session.Client.WriteLine("No connections.");
            else {
                session.Client.WriteLine("Established Connections");
                session.Client.WriteLine("########################");
                foreach (var client in server.Lp.Connections.Keys)
                    session.Client.WriteLineCentered("#", client, "#", 24);
                session.Client.WriteLine("########################");
            }
        }
    }
}
