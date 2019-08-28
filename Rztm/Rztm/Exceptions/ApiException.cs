using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rztm.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
