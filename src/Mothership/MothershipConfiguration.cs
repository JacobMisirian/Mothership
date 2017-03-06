using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Mothership
{
    public class MothershipConfiguration
    {
        public int TelnetPort { get; set; }
        public string TelnetUser { get; set; }
        public string TelnetPassword { get; set; }

        public int ClientPort { get; private set; }
        public X509Certificate SslCertificate { get; private set; }

        public static MothershipConfiguration FromFile(string path)
        {
            var config = new MothershipConfiguration();
            StreamReader reader = new StreamReader(path);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string[] parts = reader.ReadLine().Trim().Split(' ');
                string cmd = parts[0];
                string[] args = parts.Skip(1).ToArray();

                switch (cmd.ToLower())
                {
                    case "telnetport":
                        config.TelnetPort = Convert.ToInt32(args[1]);
                        break;
                    case "telnetuser":
                        config.TelnetUser = args[1];
                        break;
                    case "telnetpass":
                        config.TelnetPassword = args[1];
                        break;
                    case "clientport":
                        config.ClientPort = Convert.ToInt32(args[1]);
                        break;
                    case "sslcert":
                        config.SslCertificate = new X509Certificate2(args[1], args[2]);
                        break;
                }
            }
            return config;
        }
    }
}
