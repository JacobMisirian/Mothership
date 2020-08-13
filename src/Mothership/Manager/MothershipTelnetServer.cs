using Mothership.Lp;
using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mothership.Manager {
    public class MothershipTelnetServer {
        public MothershipLp Lp { get; private set; }

        public Dictionary<string, MothershipTelnetSession> Sessions;

        public Dictionary<string, IServerCommand> ServerCommands { get; private set; }
        public Dictionary<string, IBuiltinCommand> BuiltinCommands { get; private set; }
        public Dictionary<string, IClientMacro> ClientMacros { get; private set; }

        private Server server;

        public MothershipTelnetServer(int port, MothershipLp lp) {
            Lp = lp;

            Sessions = new Dictionary<string, MothershipTelnetSession>();

            ServerCommands = new Dictionary<string, IServerCommand>();
            BuiltinCommands = new Dictionary<string, IBuiltinCommand>();
            ClientMacros = new Dictionary<string, IClientMacro>();

            // Load all of the commands and macros from this assembly
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
                if (type.GetInterface(typeof(IServerCommand).FullName) != null) {
                    var command = (IServerCommand)Activator.CreateInstance(type);
                    ServerCommands.Add(command.Name, command);
                } else if (type.GetInterface(typeof(IBuiltinCommand).FullName) != null) {
                    var command = (IBuiltinCommand)Activator.CreateInstance(type);
                    BuiltinCommands.Add(command.Name, command);
                } else if (type.GetInterface(typeof(IClientMacro).FullName) != null) {
                    var command = (IClientMacro)Activator.CreateInstance(type);
                    ClientMacros.Add(command.Name, command);
                }
            }

            server = new Server(port);
            server.ClientConnectedEvent += server_clientConnected;
            server.ClientDisconnectedEvent += server_clientDisconnected;
        }

        public void Start() {
            server.Start();
        }

        public void EndSession(string id) {
            if (Sessions.ContainsKey(id)) {
                Sessions[id].Stop();
                Sessions.Remove(id);
            }
        }

        private void server_clientConnected(object sender, ClientConnectedEventArgs e) {
            MothershipTelnetSession session = new MothershipTelnetSession(this, e.Client);

            if (!session.Authenticate()) {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(e.Client));
            }

            session.StartInThread();
            Sessions.Add(session.Client.Id, session);
        }

        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e) {
            EndSession(e.Client.Id);
        }
    }
}
