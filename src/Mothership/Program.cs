using Mothership.ClientServer;
using Mothership.TelnetServer;

namespace Mothership
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = MothershipConfiguration.FromFile(args[0]);
            var clientServer = new ClientServer.ClientServer(config);
            clientServer.Start();

            var telnetServer = new TelnetServer.TelnetServer(config, clientServer);
            telnetServer.Start();
        }
    }
}