using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.TelnetServer
{
    public class TelnetSession
    {
        public const char PROMPT_SERVER = '%';
        public const char PROMPT_CLIENT = '$';
        public const char PROMPT_SHELL = '#';

        public string UID { get; private set; }
        public AccessLevel AccessLevel { get; private set; }
        public Char PromptLetter { get; private set; }

        public string SelectedClient { get; private set; }

        public TelnetSession(string uid)
        {
            UID = uid;
            AccessLevel = AccessLevel.Server;
            PromptLetter = PROMPT_SERVER;

            SelectedClient = string.Empty;
        }

        public void ChangeAccessLevel(AccessLevel level)
        {
            AccessLevel = level;

            switch (level)
            {
                case AccessLevel.Server:
                    PromptLetter = PROMPT_SERVER;
                    break;
                case AccessLevel.Client:
                    PromptLetter = PROMPT_CLIENT;
                    break;
                case AccessLevel.Shell:
                    PromptLetter = PROMPT_SHELL;
                    break;
            }
        }

        public void SelectClient(string client)
        {
            SelectedClient = client;
            ChangeAccessLevel(AccessLevel.Client);
        }
    }

    public enum AccessLevel
    {
        Server,
        Client,
        Shell
    }
}
