using Mothership.Lp;
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
            MothershipLp lp = new MothershipLp(1338);
            lp.Start();
            MothershipTelnetServer telnetServer = new MothershipTelnetServer(1337, lp);
            telnetServer.Start();
        }
    }
}
