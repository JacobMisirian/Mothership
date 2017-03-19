using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

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

        private bool sendingCommand = false;
        public ClientResponse SendCommand(string uid, string cmd)
        {
            try
            {
                sendingCommand = true;
                if (!Clients.ContainsKey(uid))
                    return new ClientResponse(true, string.Format("No such uid {0}!", uid));

                SendEncrypted(Clients[uid], cmd);
                string response;
                while ((response = ReadEncrypted(Clients[uid])) == "PONG")
                    Clients[uid].Pong = true;
                return new ClientResponse(false, response);
            }
            catch (Exception ex)
            {
                return new ClientResponse(true, ex.ToString());
            }
            finally
            {
                sendingCommand = false;
            }
        }

        public void SendEncrypted(TcpClient client, string msg)
        {
            byte[] msgData = ASCIIEncoding.ASCII.GetBytes(msg);
            byte[] msgEncrypted = AES.Encrypt(aesKey, aesIV, msgData);
            client.WriteLine(Convert.ToBase64String(msgEncrypted));
        }

        public string ReadEncrypted(TcpClient client)
        {
            byte[] respEncrypted = Convert.FromBase64String(client.ReadLine());
            byte[] respData = AES.Decrypt(aesKey, aesIV, respEncrypted);
            return ASCIIEncoding.ASCII.GetString(respData);
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

        private void pingThread(TcpClient client)
        {
            try
            {
                while (true)
                {
                    client.Pong = false;
                    SendEncrypted(client, "_PING");
                    Thread.Sleep(5000);
                    while (sendingCommand) ;
                    if (client.Pong)
                        continue;
                    if (ReadEncrypted(client) == "PONG")
                        client.Pong = true;
                    if (!client.Pong)
                        server_clientDisconnected(null, new ClientDisconnectedEventArgs(client));
                }
            }
            catch
            {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(client));
            }
        }

        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            e.Client.Banner = ReadEncrypted(e.Client);
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(e.Client.Banner + e.Client.IP));
            StringBuilder hexHash = new StringBuilder();
            for (int i = 0; i < CLIENT_UID_LENGTH; i++)
                hexHash.AppendFormat(hash[i].ToString("X2"));

            e.Client.UID = hexHash.ToString();
            Clients.Add(e.Client.UID, e.Client);

            e.Client.PingThread = new Thread(() => pingThread(e.Client));
            e.Client.PingThread.Start();
        }
        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            try
            {
                if (Clients.ContainsKey(e.Client.UID))
                    Clients.Remove(e.Client.UID);
                e.Client.PingThread.Abort();
                e.Client.Close();
            }
            catch
            { }
        }
    }
}
