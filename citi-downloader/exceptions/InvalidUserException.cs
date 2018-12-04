using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.exceptions
{
    public class InvalidUserException : Exception
    {
        public InvalidUserException(string message) : base(message)
        {
        }
    }
}
