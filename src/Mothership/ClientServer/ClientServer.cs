using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;

using Mothership.Networking;

namespace Mothership.ClientServer
{
    public class ClientServer
    {
        public Dictionary<string, TcpClient> Clients { get; private set; }

        private TcpServer server;

        private Stack<string> messageStack;

        public ClientServer(int port)
        {
            server = new TcpServer(port);

            messageStack = new Stack<string>();
        }
        
        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            
        }
        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

        }
        private void server_clientMessageReceived(object sender, ClientMessageReceivedEventArgs e)
        {

        }
    }
}
