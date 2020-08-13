using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mothership.Lp {
    public class MothershipConnection {
        public Client Client { get; private set; }

        public string MachineName { get; private set; }
        public string OperatingSystem { get; private set; }
        public string Username { get; private set; }

        public bool WaitingForPong { get; set; }

        private Thread pingThread;

        private MothershipLp lp;

        public MothershipConnection(MothershipLp lp, Client client, string machineName, string os, string username) {
            this.lp = lp;
            Client = client;

            MachineName = machineName;
            OperatingSystem = os;
            Username = username;

            pingThread = new Thread(() => checkConnection());
           // pingThread.Start();
        }

        public void checkConnection() {
            while (true) {
                WaitingForPong = true;
                Client.WriteLine("_PING");
                Thread.Sleep(5000);
                if (Client.ReadLine() == "PONG") {
                    WaitingForPong = false;
                }

                if (WaitingForPong) {
                    lp.EndConnection(Client.Id);
                }
            }
        }

        public string Query(string query) {
            Client.WriteLine(query);

            return Client.ReadLine();
        }
    }
}
