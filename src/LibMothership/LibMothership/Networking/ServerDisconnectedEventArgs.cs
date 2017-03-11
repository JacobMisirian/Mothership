using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMothership.Networking
{
    public class ServerDisconnectedEventArgs : EventArgs
    {
        public MothershipConnection MothershipConnection { get; private set; }

        public ServerDisconnectedEventArgs(MothershipConnection connection)
        {
            MothershipConnection = connection;
        }
    }
}
