using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rztm.Exceptions
{
    public class ConnectionException : Exception
    {


        public ConnectionException(string message) : base(message)
        {
        }
    }
}
