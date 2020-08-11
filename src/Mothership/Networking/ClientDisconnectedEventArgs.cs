using System;

namespace Mothership.Networking {
    public class ClientDisconnectedEventArgs : EventArgs {
        public Client Client { get; private set; }
        public ClientDisconnectedEventArgs(Client client) {
            Client = client;
        }
    }
}
