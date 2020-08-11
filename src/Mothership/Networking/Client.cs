using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Mothership.Networking
{
    public class Client
    {
        public string Id { get; set; }
        public string IpAddress { get { return ((IPEndPoint)BaseTcpClient.Client.RemoteEndPoint).Address.ToString(); } }
        public TcpClient BaseTcpClient { get; private set; }

        private StreamReader reader;
        private StreamWriter writer;

        public Client(TcpClient tcpClient) {
            BaseTcpClient = tcpClient;
        }

        public void Close() {
            BaseTcpClient.Close();
        }

        private bool reading = false;
        public string ReadLine() {
            while (reading) Thread.Sleep(20);
            reading = true;
            try {
                return reader.ReadLine();
            } finally {
                reading = false;
            }
        }

        private bool writing = false;
        public void Write(string strf, params object[] args) {
            while (writing) ;
            writing = true;
            try {
                if (args.Length == 0)
                    writer.Write(strf);
                else
                    writer.Write(string.Format(strf, args));
                writer.Flush();
            } finally {
                writing = false;
            }

        }
        public void WriteLine(string strf = "", params object[] args) {
            if (args.Length == 0)
                Write(strf + "\r\n");
            else
                Write(string.Format(strf + "\r\n", args));
        }

        public void WriteLineCentered(string pre, string content, string suf, int length) {
            if (pre.Length + content.Length + suf.Length >= length)
                Write("{0} {1} {2}", pre, content, suf);
            else {
                int spaces = length - (pre.Length + content.Length + suf.Length);
                int preSpace, sufSpace;

                if (spaces % 2 == 0) {
                    preSpace = spaces / 2;
                    sufSpace = spaces / 2;
                } else {
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
