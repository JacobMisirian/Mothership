using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.ClientServer
{
    public class ClientResponse
    {
        public bool Failed { get; private set; }
        public string Message { get; private set; }

        public ClientResponse(bool failed, string message)
        {
            Failed = failed;
            Message = message;
        }
    }
}
