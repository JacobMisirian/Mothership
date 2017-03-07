using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Mothership.ClientServer;
using Mothership.Networking;

namespace Mothership.TelnetServer
{
    public class TelnetServer
    {
        private string telnetUser;
        private string telnetPass;
        private string motd;

        private TcpServer server;
        private ClientServer.ClientServer clientServer;

        private Dictionary<string, TcpClient> users;
        private Dictionary<string, TelnetSession> sessions;

        public TelnetServer(string telnetUser, string telnetPass, int port, string motd, ClientServer.ClientServer clientServer)
        {
            this.telnetUser = telnetUser;
            this.telnetPass = telnetPass;
            this.motd = motd;

            server = new TcpServer(port);
            this.clientServer = clientServer;

            users = new Dictionary<string, TcpClient>();
            sessions = new Dictionary<string, TelnetSession>();
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

        private void startSessionThread(TcpClient user, TelnetSession session)
        {
            try
            {
                while (true)
                {
                    user.Write(session.GetPrompt());
                    handleMessage(user, session, user.ReadLine());
                }
            }
            catch (IOException)
            {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(user));
            }
            catch (NullReferenceException)
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
            }
            catch (IOException)
            {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(user));
            }
            catch (NullReferenceException)
            {
                server_clientDisconnected(null, new ClientDisconnectedEventArgs(user));
            }
        }

        private int sessionNumber = 0;
        private void server_clientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (!handleLogin(e.Client))
                return;

            string uid = sessionNumber++.ToString();
            e.Client.UID = uid;

            users.Add(uid, e.Client);

            var session = new TelnetSession(uid);
            session.SessionThread = new Thread(() => startSessionThread(e.Client, session));
            session.SessionThread.Start();
            sessions.Add(uid, session);    
        }

        private void server_clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            try
            {
                if (sessions.ContainsKey(e.Client.UID))
                {
                    sessions[e.Client.UID].SessionThread.Abort();
                    sessions.Remove(e.Client.UID);
                }
                if (users.ContainsKey(e.Client.UID))
                    users.Remove(e.Client.UID);
                e.Client.Close();
               
            }
            catch
            {

            }
        }
    }
}
