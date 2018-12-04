using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.exceptions
{
    public class UnknownUserException : Exception
    {
        public UnknownUserException(string message) : base(message)
        {

        }
    }
}
