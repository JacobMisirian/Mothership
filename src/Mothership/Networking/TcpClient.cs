using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Mothership.Networking
{
    public class TcpClient
    {
        public string UID { get; set; }

        public System.Net.Sockets.TcpClient BaseClient {  get { return realClient; } }

        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        public Thread MessageListenerThread { get; set; }

        private System.Net.Sockets.TcpClient realClient;

        public TcpClient(System.Net.Sockets.TcpClient client, bool usingSsl = false, X509Certificate certificate = null)
        {
            realClient = client;
            if (usingSsl)
            {
                SslStream ssl = new SslStream(client.GetStream(), false);
                ssl.AuthenticateAsServer(certificate, false, System.Security.Authentication.SslProtocols.Tls, true);
                Reader = new StreamReader(ssl);
                Writer = new StreamWriter(ssl);
            }
            else
            {
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
            }
        }

        public void Close()
        {
            realClient.Close();
        }

        public string ReadLine()
        {
            return Reader.ReadLine();
        }

        public void WriteLine(string strf, params object[] args)
        {
            if (args.Length == 0)
                Writer.WriteLine(strf);
            else
                Writer.WriteLine(string.Format(strf, args));
            Writer.Flush();
        }
    }
}
