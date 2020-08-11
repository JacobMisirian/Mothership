using System;

namespace Mothership.Networking {
    public class ClientConnectedEventArgs : EventArgs {
        public Client Client { get; private set; }
        public ClientConnectedEventArgs(Client client) {
            Client = client;
        }
    }
}
