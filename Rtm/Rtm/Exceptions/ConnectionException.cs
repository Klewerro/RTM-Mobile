using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rtm.Exceptions
{
    public class ConnectionException : Exception
    {


        public ConnectionException(string message) : base(message)
        {
        }
    }
}
