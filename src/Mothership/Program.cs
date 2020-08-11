using Mothership.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership
{
    class Program
    {
        static void Main(string[] args)
        {
            MothershipTelnetServer telnetServer = new MothershipTelnetServer(1337, null);
            telnetServer.Start();
        }
    }
}
