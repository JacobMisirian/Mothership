using System;
using System.Collections.Generic;
using System.Text;

namespace Mothership.Networking
{
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public TcpClient Client { get; private set; }

        public ClientDisconnectedEventArgs(TcpClient client)
        {
            Client = client;
        }
    }
}
