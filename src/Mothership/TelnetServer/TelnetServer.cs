using System;
using System.Collections.Generic;
using System.Text;

using Mothership.Networking;

namespace Mothership.TelnetServer
{
    public class TelnetServer
    {
        private TcpServer server;

        private Dictionary<string, TcpClient> users;
        private Dictionary<string, TelnetSession> sessions;

        public TelnetServer(int port)
        {
            server = new TcpServer(port);

            users = new Dictionary<string, TcpClient>();
            sessions = new Dictionary<string, TelnetSession>();
        }

        public void Start()
        {
            server.Start();

            server.ClientConnected += server_clientConnected;
            server.ClientDisconnected += server_clientDisconnected;
            server.ClientMessageReceived += server_clientMessageReceived;
        }

        private void handleMessage(TcpClient user, string message)
        {
            string[] parts = message.Split(' ');
            string cmd = parts[0];
            
        }

        private int sessionNumber = 0;
        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            string uid = sessionNumber++.ToString();
            e.Client.UID = uid;

            users.Add(uid, e.Client);
            sessions.Add(uid, new TelnetSession(uid));
        }

        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            if (users.ContainsKey(e.Client.UID))
                users.Remove(e.Client.UID);
            if (sessions.ContainsKey(e.Client.UID))
                sessions.Remove(e.Client.UID);
            e.Client.Close();
        }

        private void server_clientMessageReceived(object sender, ClientMessageReceivedEventArgs e)
        {

        }
    }
}
