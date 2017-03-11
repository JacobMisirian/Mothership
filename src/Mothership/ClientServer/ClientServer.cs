using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

using Mothership.Networking;

namespace Mothership.ClientServer
{
    public class ClientServer
    {
        public const int CLIENT_UID_LENGTH = 0x07;

        public Dictionary<string, TcpClient> Clients { get; private set; }

        private TcpServer server;

        public ClientServer(int port, X509Certificate certificate)
        {
            server = new TcpServer(port, certificate);

            Clients = new Dictionary<string, TcpClient>();
        }

        public void Disconnect(TcpClient client)
        {
            server_clientDisconnected(null, new ClientDisconnectedEventArgs(client));
        }
        
        public ClientResponse SendCommand(string uid, string cmd)
        {
            try
            {
                if (!Clients.ContainsKey(uid))
                    return new ClientResponse(true, string.Format("No such uid {0}!", uid));
                Clients[uid].WriteLine(cmd);
                return new ClientResponse(false, ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(Clients[uid].ReadLine())));
            }
            catch (Exception ex)
            {
                return new ClientResponse(true, ex.ToString());
            }
        }

        public void Start()
        {
            server.Start();

            server.ClientConnected += server_clientConnected;
            server.ClientDisconnected += server_clientDisconnected;
        }

        public void Stop()
        {
            server.Stop();
        }

        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.Banner = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(e.Client.ReadLine()));
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(e.Client.Banner + e.Client.IP));
            StringBuilder hexHash = new StringBuilder();
            for (int i = 0; i < CLIENT_UID_LENGTH; i++)
                hexHash.AppendFormat(hash[i].ToString("X2"));

            e.Client.UID = hexHash.ToString();
            Clients.Add(e.Client.UID, e.Client);
        }
        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            try
            {
                if (Clients.ContainsKey(e.Client.UID))
                    Clients.Remove(e.Client.UID);
                e.Client.Close();
            }
            catch
            { }
        }
    }
}
