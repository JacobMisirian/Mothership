using Mothership.Lp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.Manager.BuiltinCommands {
    public class InfoCommand : IBuiltinCommand {
        public string Name { get { return "info"; } }
        public string Syntax { get { return "info"; } }

        public void Invoke(MothershipTelnetServer server, MothershipTelnetSession session, MothershipConnection target, params string[] args) {
            session.Client.WriteLine("Client '{0}':", target.Client.Id);
            session.Client.WriteLine("#######################################################");
            session.Client.WriteLine("Machine Name: '{0}': ", target.MachineName);
            session.Client.WriteLine("Operating System: '{0}': ", target.OperatingSystem);
            session.Client.WriteLine("Username: '{0}': ", target.Username);
            session.Client.WriteLine("#######################################################");
        }
    }
}
