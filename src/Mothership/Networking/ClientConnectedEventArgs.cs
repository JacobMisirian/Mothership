using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.Networking
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public TcpClient Client { get; private set; }

        public ClientConnectedEventArgs(TcpClient client)
        {
            Client = client;
        }
    }
}
