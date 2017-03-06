using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.Networking
{
    public class ClientMessageReceivedEventArgs : EventArgs
    {
        public TcpClient Client { get; private set; }
        public string Messsage { get; private set; }

        public ClientMessageReceivedEventArgs(TcpClient client, string message)
        {
            Client = client;
            Messsage = message;
        }
    }
}
