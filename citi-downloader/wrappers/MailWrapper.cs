using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace CitiDownloader.wrappers
{
    public class MailWrapper : IMailWrapper
    {
        public void SendEmail(string to, string from, string subject, string message)
        {
            SmtpClient client = new SmtpClient("mailhub.iastate.edu");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(to);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            client.Send(mailMessage);
        }
    }
}
