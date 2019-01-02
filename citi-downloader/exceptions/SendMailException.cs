using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.exceptions
{
    public class SendMailException : Exception
    {
        public SendMailException(string message) : base(message)
        {
        }
    }
}
