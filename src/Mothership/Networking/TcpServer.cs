using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Mothership.Networking
{
    public class TcpServer
    {
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ClientMessageReceivedEventArgs> ClientMessageReceived;

        public X509Certificate SslCertificate { get; private set; }
        public bool UsingSsl { get { return SslCertificate != null; } }
        
        private System.Net.Sockets.TcpListener listener;
        private Thread connectionListenerThread;

        public TcpServer(int port)
        {
            listener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
            SslCertificate = null;
        }
        public TcpServer(int port, X509Certificate certificate)
        {
            listener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
            SslCertificate = certificate;
        }

        public void Start()
        {
            listener.Start();

            connectionListenerThread = new Thread(() => listenForConnectionsThread());
            connectionListenerThread.Start();
        }

        public void Stop()
        {
            connectionListenerThread.Abort();
            listener.Stop();
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

        private void listenForMessagesThread(TcpClient client)
        {
            while (true)
                OnClientMessageReceived(client, client.ReadLine());
        }

        protected virtual void OnClientConnected(System.Net.Sockets.TcpClient client)
        {
            var handler = ClientConnected;
            if (handler != null)
            {
                var client_ = new TcpClient(client, UsingSsl, SslCertificate);
                client_.MessageListenerThread = new Thread(() => listenForMessagesThread(client_));
                client_.MessageListenerThread.Start();

                handler(this, new ClientConnectedEventArgs(client_));
            }
        }
        protected virtual void OnClientDisconnected(TcpClient client)
        {
            var handler = ClientDisconnected;
            if (handler != null)
                handler(this, new ClientDisconnectedEventArgs(client));
        }
        protected virtual void OnClientMessageReceived(TcpClient client, string message)
        {
            var handler = ClientMessageReceived;
            if (handler != null)
                handler(this, new ClientMessageReceivedEventArgs(client, message));
        }
    }
}
