using System;

using LibMothership.Networking;

namespace LibMothership
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var conn = new LibMothership.Networking.MothershipConnection(args[0], Convert.ToInt32(args[1]));
            conn.ServerMessageReceived += connection_messageReceived;
            
        }

        static void connection_messageReceived(object sender, ServerMessageReceivedEventArgs e)
        { }
    }
}
