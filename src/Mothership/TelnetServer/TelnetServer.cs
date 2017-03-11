using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Mothership.Networking;
using Mothership.TelnetServer.ServerCommands;

namespace Mothership.TelnetServer
{
    public class TelnetServer
    {
        public ClientServer.ClientServer ClientServer { get; private set; }
        public Dictionary<string, IClientCommand> ClientCommands { get; private set; }
        public Dictionary<string, IServerCommand> ServerCommands { get; private set; }

        public Dictionary<string, TelnetSession> Sessions { get; private set; }
        public Dictionary<string, TcpClient> Users { get; private set; }

        private string telnetUser;
        private string telnetPass;
        private string motd;

        private TcpServer server;


        public TelnetServer(string telnetUser, string telnetPass, int port, string motd, ClientServer.ClientServer clientServer)
        {
            ClientServer = clientServer;
            this.telnetUser = telnetUser;
            this.telnetPass = telnetPass;
            this.motd = motd;

            server = new TcpServer(port);

            Users = new Dictionary<string, TcpClient>();
            Sessions = new Dictionary<string, TelnetSession>();

            ClientCommands = new Dictionary<string, IClientCommand>();
            ServerCommands = new Dictionary<string, IServerCommand>();

            LoadClientCommands(Assembly.GetExecutingAssembly());
            LoadServerCommands(Assembly.GetExecutingAssembly());
        }

        public void DisconnectClient(TcpClient client)
        {
            ClientServer.Disconnect(client);
        }

        public void LoadClientCommands(Assembly ass)
        {
            foreach (var type in ass.GetTypes())
            {
                if (type.GetInterface(typeof(IClientCommand).FullName) != null)
                {
                    var command = (IClientCommand)Activator.CreateInstance(type);
                    ClientCommands.Add(command.Name, command);
                }
            }
        }

        public void LoadServerCommands(Assembly ass)
        {
            foreach (var type in ass.GetTypes())
            {
                if (type.GetInterface(typeof(IServerCommand).FullName) != null)
                {
                    var command = (IServerCommand)Activator.CreateInstance(type);
                    ServerCommands.Add(command.Name, command);
                }
            }
        }

        public void Start()
        {
            server.Start();

            server.ClientConnected += server_clientConnected;
            server.ClientDisconnected += server_clientDisconnected;
        }

        private bool handleLogin(TcpClient user)
        {
            user.WriteLine(motd);
            user.WriteLine();
            user.WriteLine("Press return to continue.");
            user.ReadLine();
            user.WriteLine();
            user.WriteLine();
            user.Write("Username: ");
            string enteredUser = user.ReadLine();
            user.Write("Password: ");
            string enteredPass = user.ReadLine();

            if (enteredUser != telnetUser || enteredPass != telnetPass)
            {
                user.WriteLine("Incorrect credentials!");
                user.WriteLine("Terminating connection...");
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(user));
                return false;
            }
            Thread.Sleep(300);
            user.WriteLine("\u001B[2J");
            return true;
        }

        private void sessionThread(TcpClient user, TelnetSession session)
        {
            try
            {
                while (true)
                {
                    user.Write(session.GetPrompt());
                    string input = user.ReadLine();
                    if (input.Trim() != string.Empty)
                        handleMessage(user, session, input);
                }
            }
            catch (Exception)
            {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(user));
            }
        }

        private void handleMessage(TcpClient user, TelnetSession session, string message)
        {
            try
            {
                string[] parts = message.Split(' ');
                string cmd = parts[0];
                string[] args = parts.Skip(1).ToArray();

                switch (session.AccessLevel)
                {
                    case AccessLevel.Server:
                        if (ServerCommands.ContainsKey(cmd))
                            ServerCommands[cmd].Invoke(this, user, session, args);
                        else
                            user.WriteLine("No such command {0}! Type 'help' for help.", cmd);
                        break;
                    case AccessLevel.Client:
                        if (ClientCommands.ContainsKey(cmd))
                            ClientCommands[cmd].Invoke(this, user, session, args);
                        else
                            user.WriteLine("No such command {0}! Type 'help' for help.", cmd);
                        break;
                    case AccessLevel.Shell:
                        if (cmd == "exit")
                            session.ChangeAccessLevel(AccessLevel.Client);
                        else
                        {
                            var response = ClientServer.SendCommand(session.SelectedClient, message);
                            if (response.Failed)
                                user.WriteLine("Failed! Reason: {0}", response.Message);
                            else
                                user.WriteLine(response.Message);
                        }
                        break;
                }
            }
            catch (ArgumentLengthException ex)
            {
                user.WriteLine("{0} expected {1} argument(s), instead got {2}!", ex.Target, ex.Expected, ex.InvokedWith);
            }
            catch (Exception ex)
            {
                user.WriteLine("Error! Reason: {0}", ex.ToString());
            }
        }

        private int sessionNumber = 0;
        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (!handleLogin(e.Client))
                return;

            string uid = sessionNumber++.ToString();
            e.Client.UID = uid;

            Users.Add(uid, e.Client);

            var session = new TelnetSession(uid);
            session.SessionThread = new Thread(() => sessionThread(e.Client, session));
            session.SessionThread.Start();
            Sessions.Add(uid, session);    
        }

        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            try
            {
                if (Sessions.ContainsKey(e.Client.UID))
                {
                    Sessions[e.Client.UID].SessionThread.Abort();
                    Sessions.Remove(e.Client.UID);
                }
                if (Users.ContainsKey(e.Client.UID))
                    Users.Remove(e.Client.UID);
                e.Client.Close();
               
            }
            catch
            {

            }
        }
    }
}
