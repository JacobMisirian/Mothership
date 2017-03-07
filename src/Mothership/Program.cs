using Mothership.ClientServer;
using Mothership.TelnetServer;

namespace Mothership
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = MothershipConfiguration.FromFile(args[0]);
            var clientServer = new ClientServer.ClientServer(config.ClientPort);
            clientServer.Start();

            var telnetServer = new TelnetServer.TelnetServer(config.TelnetUser, config.TelnetPassword, config.TelnetPort, config.TelnetMotd, clientServer);
            telnetServer.Start();
        }
    }
}