using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface IMailWrapper
    {
        void SendEmail(string to, string from, string subject, string message);
    }
}
