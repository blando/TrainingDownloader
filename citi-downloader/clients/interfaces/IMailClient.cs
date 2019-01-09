using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.wrappers
{
    public interface IMailClient
    {
        void SendEmail(string to, string from, string subject, string message);
    }
}
