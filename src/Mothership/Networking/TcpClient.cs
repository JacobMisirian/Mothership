using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Mothership.Networking
{
    public class TcpClient
    {
        public string Banner { get; set; }
        public string UID { get; set; }
        public string IP {  get { return ((IPEndPoint)BaseClient.Client.RemoteEndPoint).Address.ToString(); } }

        public System.Net.Sockets.TcpClient BaseClient {  get { return realClient; } }

        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        private System.Net.Sockets.TcpClient realClient;

        public TcpClient(System.Net.Sockets.TcpClient client, bool usingSsl = false, X509Certificate certificate = null)
        {
            UID = string.Empty;
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

        public void Write(string strf, params object[] args)
        {
            if (args.Length == 0)
                Writer.Write(strf);
            else
                Writer.Write(string.Format(strf, args));
            Writer.Flush();

        }
        public void WriteLine(string strf = "", params object[] args)
        {
            Write(string.Format(strf + "\r\n", args));
        }
    }
}
