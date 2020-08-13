using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        private Thread pongThread;

        private MothershipLp lp;

        public MothershipConnection(MothershipLp lp, Client client, string machineName, string os, string username) {
            this.lp = lp;
            Client = client;

            MachineName = machineName;
            OperatingSystem = os;
            Username = username;

            pingThread = new Thread(() => sendPings());
            pongThread = new Thread(() => getPongs());
            
            pingThread.Start();
            pongThread.Start();
        }

        private void sendPings() {
            try {
                while (true) {
                    Client.WriteLine("_PING");
                    WaitingForPong = true;
                    Thread.Sleep(10000);

                    if (WaitingForPong) {
                        lp.EndConnection(Client.Id);
                    }

                }
            } catch {
                lp.EndConnection(Client.Id);
            }
        }

        private void getPongs() {
            try {
                while (true) {
                    while (!WaitingForPong) Thread.Sleep(20);
                    Client.ReadLine();
                    WaitingForPong = false;
                }
            } catch {
                lp.EndConnection(Client.Id);
            }
        }

        public string Query(string query) {
            try {
                Client.WriteLine(query);

                return Client.ReadLine();
            } catch {
                lp.EndConnection(Client.Id);
                return string.Empty;
            }
        }
    }
}
