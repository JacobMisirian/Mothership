using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMothership.Networking
{
    public class ServerMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public ServerMessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
