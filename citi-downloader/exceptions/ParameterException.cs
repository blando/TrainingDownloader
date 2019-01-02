using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.exceptions
{
    public class ParameterException : Exception
    {
        public ParameterException(string message) : base(message)
        {
        }
    }
}
