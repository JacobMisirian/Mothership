using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Mothership.Networking;

namespace Mothership.ClientServer
{
    public class ClientServer
    {
        public Dictionary<string, TcpClient> Clients { get; private set; }

        private TcpServer server;

        public ClientServer(int port)
        {
            server = new TcpServer(port);

            Clients = new Dictionary<string, TcpClient>();
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
            e.Client.Banner = e.Client.ReadLine();
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(e.Client.Banner + e.Client.IP));
            e.Client.UID = ASCIIEncoding.ASCII.GetString(hash).Substring(6);

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
