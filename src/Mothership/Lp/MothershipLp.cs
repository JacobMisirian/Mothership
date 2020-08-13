using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Mothership.Lp {
    public class MothershipLp {
        public Dictionary<string, MothershipConnection> Connections { get; private set; }

        private Server server;

        public MothershipLp(int port) {
            Connections = new Dictionary<string, MothershipConnection>();

            server = new Server(port);
            server.ClientConnectedEvent += server_clientConnected;
            server.ClientDisconnectedEvent += server_clientDisconnected;
        }

        public void Start() {
            server.Start();
        }

        public void Stop() {
            server.Stop();
        }

        public void EndConnection(string id) {
            if (Connections.ContainsKey(id)) {
                Connections[id].Client.Close();
                Connections.Remove(id);
            }
        }

        private void server_clientConnected(object sender, ClientConnectedEventArgs e) {
            MothershipConnection c = new MothershipConnection(this, e.Client, e.Client.ReadLine(), e.Client.ReadLine(), e.Client.ReadLine());

            // Calculate ID based on MD5 hash of banner.
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(c.MachineName + c.OperatingSystem + c.Username + c.Client.IpAddress));
            StringBuilder strHash = new StringBuilder();
            for (int i = 0; i < 5; i++) {
                strHash.AppendFormat(hash[i].ToString("X2"));
            }

            c.Client.Id = strHash.ToString();

            Connections.Add(c.Client.Id, c);
        }

        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e) {
            EndConnection(e.Client.Id);
        }
    }
}
