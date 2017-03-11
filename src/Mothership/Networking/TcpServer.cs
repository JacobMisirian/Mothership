using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Mothership.Networking
{
    public class TcpServer
    {
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        public X509Certificate SslCertificate { get; private set; }
        public bool UsingSsl { get { return SslCertificate != null; } }
        
        private System.Net.Sockets.TcpListener listener;
        private Thread connectionListenerThread;
        private Thread connectionCheckerThread;

        private List<TcpClient> internalClientList;

        public TcpServer(int port)
        {
            listener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
            SslCertificate = null;
            internalClientList = new List<TcpClient>();
        }
        public TcpServer(int port, X509Certificate certificate)
        {
            listener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
            SslCertificate = certificate;
            internalClientList = new List<TcpClient>();
        }

        public void Start()
        {
            listener.Start();

            connectionListenerThread = new Thread(() => listenForConnectionsThread());
            connectionListenerThread.Start();

            connectionCheckerThread = new Thread(() => checkConnectionsThread());
            connectionCheckerThread.Start();
        }

        public void Stop()
        {
            connectionListenerThread.Abort();
            connectionCheckerThread.Abort();
            listener.Stop();
        }

        private void checkConnectionsThread()
        {
            while (true)
            {
                TcpClient currentClient = null;
                try
                {
                    Thread.Sleep(50);
                    var disconnected = new List<TcpClient>();
                    foreach (var client in internalClientList)
                    {
                        currentClient = client;
                        if (client.BaseClient.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] checkConn = new byte[1];
                            if (client.BaseClient.Client.Receive(checkConn, SocketFlags.Peek) == 0)
                                disconnected.Add(client);
                        }
                    }
                    for (int i = 0; i < disconnected.Count; i++)
                        OnClientDisconnected(disconnected[i]);
                }
                catch (Exception)
                {
                    if (currentClient != null)
                        OnClientDisconnected(currentClient);
                }
            }
        }

        private void listenForConnectionsThread()
        {
            while (true)
            {
                try
                {
                    OnClientConnected(listener.AcceptTcpClient());
                }
                catch
                { }
            }
        }

        protected virtual void OnClientConnected(System.Net.Sockets.TcpClient client)
        {

            var handler = ClientConnected;
            if (handler != null)
            {
                var client_ = new TcpClient(client, UsingSsl, SslCertificate);
                internalClientList.Add(client_);
                handler(this, new ClientConnectedEventArgs(client_));
            }
        }
        protected virtual void OnClientDisconnected(TcpClient client)
        {
            internalClientList.Remove(client);

            var handler = ClientDisconnected;
            if (handler != null)
                handler(this, new ClientDisconnectedEventArgs(client));
        }
    }
}
