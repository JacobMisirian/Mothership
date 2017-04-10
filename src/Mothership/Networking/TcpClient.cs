using System;
using System.IO;
using System.Linq;
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

        public Thread PingThread { get; set; }
        public bool Pong { get; set; }

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
            Pong = false;
        }

        public void Close()
        {
            realClient.Close();
        }

        private bool reading = false;
        public string ReadLine()
        {
            while (reading) ;
            reading = true;
            try
            {
                return Reader.ReadLine();
            }
            finally
            {
                reading = false;
            }
        }

        private bool writing = false;
        public void Write(string strf, params object[] args)
        {
            while (writing) ;
            writing = true;
            try
            {
                if (args.Length == 0)
                    Writer.Write(strf);
                else
                    Writer.Write(string.Format(strf, args));
                Writer.Flush();
            }
            finally
            {
                writing = false;
            }

        }
        public void WriteLine(string strf = "", params object[] args)
        {
            if (args.Length == 0)
                Write(strf + "\r\n");
            else
                Write(string.Format(strf + "\r\n", args));
        }

        public void WriteLineCentered(string pre, string content, string suf, int length)
        {
            if (pre.Length + content.Length + suf.Length >= length)
                Write("{0} {1} {2}", pre, content, suf);
            else
            {
                int spaces = length - (pre.Length + content.Length + suf.Length);
                int preSpace, sufSpace;

                if (spaces % 2 == 0)
                {
                    preSpace = spaces / 2;
                    sufSpace = spaces / 2;
                }
                else
                {
                    preSpace = (spaces - 1) / 2;
                    sufSpace = (spaces + 1) / 2;
                }

                Write(pre);
                Write(string.Concat(Enumerable.Repeat(" ", preSpace)));
                Write(content);
                Write(string.Concat(Enumerable.Repeat(" ", sufSpace)));
                WriteLine(suf);
            }
        }
    }
}
