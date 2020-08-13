using Mothership.Lp;
using Mothership.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mothership.Manager {
    public class MothershipTelnetSession {
        private static int nextTelnetSessionId = 0;
 
        private MothershipTelnetServer server;

        public Client Client { get; private set; }
        public TelnetUserLevel UserLevel { get; set; }
        public MothershipConnection SelectedClient { get; set; }

        public MothershipTelnetSession(MothershipTelnetServer server, Client client) {
            this.server = server;

            Client = client;
            client.Id = (nextTelnetSessionId++).ToString();

            UserLevel = TelnetUserLevel.Server;
        }

        public bool Authenticate() {
            Client.WriteLine();
            Client.WriteLine("Press return to continue.");
            Client.ReadLine();
            Client.WriteLine();
            Client.WriteLine();
            Client.Write("User: ");
            string enteredUser = Client.ReadLine();
            Client.Write("Password: ");
            string enteredPass = Client.ReadLine();

            if (enteredUser != "root" || enteredPass != "root") {
                Client.WriteLine("Incorrect credentials!");
                Client.WriteLine("Terminating connection...");
                return false;
            }
            Thread.Sleep(300);
            Client.WriteLine("\u001B[2J");
            return true;
        }


        private Thread promptThread;
        public void StartInThread() {
            if (promptThread != null) {
                promptThread.Abort();
                promptThread = null;
            }

            promptThread = new Thread(() => start());
            promptThread.Start();
        }

        private void start() {
            try {
                while (true) {
                    Client.Write(formatShellPrompt());

                    string input = Client.ReadLine();
                    if (input.Trim() != string.Empty) {
                        processInput(input);
                    }
                }
            } catch (Exception ex) {
               // Console.WriteLine(ex.ToString());
            } finally {
                Stop();
            }
        }

        public void Stop() {
            if (promptThread != null) {
                promptThread.Abort();
                promptThread = null;
            }

            Client.Close();
        }

        private void processInput(string input) {
            try {
                string[] parts = input.Split(' ');
                string cmd = parts[0];
                string[] args = parts.Skip(1).ToArray();

                switch (UserLevel) {
                    case TelnetUserLevel.Server:
                        if (server.ServerCommands.ContainsKey(cmd)) {
                            server.ServerCommands[cmd].Invoke(server, this, args);
                        } else {
                            Client.WriteLine("No such command {0}! Type help for help.", cmd);
                        }
                        break;

                    case TelnetUserLevel.Client:
                        if (server.BuiltinCommands.ContainsKey(cmd)) {
                            server.BuiltinCommands[cmd].Invoke(server, this, SelectedClient, args);
                        } else {
                            Client.WriteLine("No such command {0}! Type help for help.", cmd);
                        }
                        break;
                    case TelnetUserLevel.Interactive:
                        Client.WriteLine(SelectedClient.Query(input));
                        break;
                }
            } catch (ArgumentNullException ane) {

            } catch (ArgumentException ae) {

            } catch (Exception ex) {
                Client.WriteLine(ex.ToString());
            }
        }

        private string formatShellPrompt() {
            return string.Format("{0}{1} ", UserLevel == TelnetUserLevel.Server ? string.Empty : SelectedClient.Client.Id, ((char)UserLevel).ToString());
        }
    }
}
