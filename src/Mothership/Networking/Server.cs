using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Mothership.Networking {
    public class Server {
        public event EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnectedEvent;

        private TcpListener listener;
        private List<Client> connectedClients;

        private Thread connectionListenerThread;
        private Thread connectionCheckerThread;

        public Server(int port) {
            listener = new TcpListener(IPAddress.Any, port);
            connectedClients = new List<Client>();
        }

        public void Start() {
            listener.Start();

            connectionListenerThread = new Thread(() => listenForConnections());
            connectionListenerThread.Start();

            connectionCheckerThread = new Thread(() => checkConnections());
            connectionCheckerThread.Start();
        }

        public void Stop() {
            connectionListenerThread.Abort();
            connectionCheckerThread.Abort();
            listener.Stop();
        }

        private void checkConnections() {
            while (true) {
                Client currentClient = null;
                try {
                    Thread.Sleep(50);
                    var disconnected = new List<Client>();
                    foreach (var client in connectedClients) {
                        currentClient = client;
                        if (client.BaseTcpClient.Client.Poll(0, SelectMode.SelectRead)) {
                            byte[] checkConn = new byte[1];
                            if (client.BaseTcpClient.Client.Receive(checkConn, SocketFlags.Peek) == 0)
                                disconnected.Add(client);
                        }
                    }
                    for (int i = 0; i < disconnected.Count; i++)
                        OnClientDisconnected(disconnected[i]);
                } catch (Exception) {
                    if (currentClient != null)
                        OnClientDisconnected(currentClient);
                }
            }
        }

        private void listenForConnections() {
            while (true) {
                try {
                    OnClientConnected(listener.AcceptTcpClient());
                } catch { }
            }
        }

        protected virtual void OnClientConnected(TcpClient tcpClient) {
            var handler = ClientConnectedEvent;
            if (handler != null) {
                var client = new Client(tcpClient);
                connectedClients.Add(client);
                handler(this, new ClientConnectedEventArgs(client));
            }
        }
        protected virtual void OnClientDisconnected(Client client) {
            connectedClients.Remove(client);

            var handler = ClientDisconnectedEvent;
            if (handler != null)
                handler(this, new ClientDisconnectedEventArgs(client));
        }
    }
}
