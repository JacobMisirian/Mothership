using Mothership.ClientServer;
using Mothership.TelnetServer;

namespace Mothership
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = MothershipConfiguration.FromFile(args[0]);
            var clientServer = new ClientServer.ClientServer(config.ClientPort, config.SslCertificate);
            if (config.SmsServer != string.Empty)
                clientServer.RegisterSmsNumbers(config.SmsServer, config.SmsPort, config.SmsSimNumber, config.SmsNumbers);
            clientServer.Start();

            var telnetServer = new TelnetServer.TelnetServer(config.TelnetUser, config.TelnetPassword, config.TelnetPort, config.TelnetMotd, clientServer);
            if (config.SmsServer != string.Empty)
                telnetServer.RegisterSmsNumbers(config.SmsServer, config.SmsPort, config.SmsSimNumber, config.SmsNumbers);
            telnetServer.Start();
        }
    }
}