using System;
using System.Reflection;

using LibMothership;
using LibMothership.Networking;

namespace LibMothership.Test
{
    class Program
    {
        static MothershipConnection connection;

        static void Main(string[] args)
        {
            connection = new MothershipConnection(args[0], Convert.ToInt32(args[1]));
            connection.LoadCommandsFromAssembly(Assembly.GetExecutingAssembly());
            connection.ServerConnected += connection_serverConnected;
            connection.ServerDisconnected += connection_serverDisconnected;
            connection.ServerMessageReceived += connection_serverMessageReceived;
            connection.Start();
        }

        static void connection_serverConnected(object sender, ServerConnectedEventArgs e)
        {
            Console.WriteLine("Connected!");
        }
        static void connection_serverDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected!");
        }
        static void connection_serverMessageReceived(object sender, ServerMessageReceivedEventArgs e)
        {
            Console.WriteLine("Message Received! Not picked up by handler:");
            Console.WriteLine(e.Message);
            Console.Write("Response? ");
            connection.Send(Console.ReadLine());
        }
    }
}
