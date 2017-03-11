using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMothership.Networking
{
    public class ServerConnectedEventArgs : EventArgs
    {
        public MothershipConnection MothershipConnection { get; private set; }

        public ServerConnectedEventArgs(MothershipConnection connection)
        {
            MothershipConnection = connection;
        }
    }
}
