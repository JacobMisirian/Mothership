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
        public string TelnetMotd { get; set; }

        public int ClientPort { get; private set; }
        public X509Certificate SslCertificate { get; private set; }

        public MothershipConfiguration()
        {
            TelnetPort = 23;
            TelnetUser = "root";
            TelnetPassword = "toor";
            TelnetMotd = string.Empty;
        }

        public static MothershipConfiguration FromFile(string path)
        {
            var config = new MothershipConfiguration();
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split(' ');
                string cmd = parts[0];
                string[] args = parts.Skip(1).ToArray();

                switch (cmd.ToLower())
                {
                    case "telnetport":
                        config.TelnetPort = Convert.ToInt32(args[0]);
                        break;
                    case "telnetuser":
                        config.TelnetUser = args[0];
                        break;
                    case "telnetpass":
                        config.TelnetPassword = args[0];
                        break;
                    case "clientport":
                        config.ClientPort = Convert.ToInt32(args[0]);
                        break;
                    case "sslcert":
                        config.SslCertificate = new X509Certificate2(args[0], args[1]);
                        break;
                    case "motd":
                        config.TelnetMotd = string.Join("\r\n", File.ReadAllLines(args[0]));
                        break;
                }
            }
            return config;
        }
    }
}
