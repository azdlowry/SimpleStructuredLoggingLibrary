using System;
using System.Linq;

namespace SimpleStructuredLoggingLibrary
{

    public class InvalidLogMessageTypeException : ArgumentException
    {
        public InvalidLogMessageTypeException(string message, string paramName) : base(message, paramName)
        {
        }
    }
}
