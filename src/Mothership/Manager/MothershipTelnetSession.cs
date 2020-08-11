using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mothership.Manager {
    public class MothershipTelnetSession {
        private static int nextTelnetSessionId = 0;

        public Client Client { get; private set; }
       
        public MothershipTelnetSession(Client client) {
            Client = client;
            client.Id = (nextTelnetSessionId++).ToString();
        }

        public bool Authenticate() {
            Client.WriteLine();
            Client.WriteLine("Press return to continue.");
            Client.ReadLine();
            Client.WriteLine();
            Client.WriteLine();
            Client.Write("Clientname: ");
            string enteredClient = Client.ReadLine();
            Client.Write("Password: ");
            string enteredPass = Client.ReadLine();

            if (enteredClient != "root" || enteredPass != "root") {
                Client.WriteLine("Incorrect credentials!");
                Client.WriteLine("Terminating connection...");
                return false;
            }
            Thread.Sleep(300);
            Client.WriteLine("\u001B[2J");
            return true;
        }


        private Thread runThread;
        public void StartInThread() {
            Stop();

            runThread = new Thread(() => start());
            runThread.Start();
        }

        private void start() {

        }

        public void Stop() {
            if (runThread != null) {
                runThread.Abort();
                runThread = null;
            }

            Client.Close();
        }
    }
}
