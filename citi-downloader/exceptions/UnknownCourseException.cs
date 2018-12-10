using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.exceptions
{
    public class UnknownCourseException : Exception
    {
        public UnknownCourseException(string message) : base(message)
        {
        }
    }
}
