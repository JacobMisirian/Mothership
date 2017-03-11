using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using LibMothership.Networking;

namespace LibMothership
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = new MothershipConnection(args[0], Convert.ToInt32(args[1]));

            new Thread(() => readThread(conn)).Start();
            while (true)
                conn.Send(Console.ReadLine());
        }

        static void readThread(MothershipConnection conn0)
        {
            while (true)
                Console.WriteLine(conn0.Read());
        }
    }
}
