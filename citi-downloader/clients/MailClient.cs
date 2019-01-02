using CitiDownloader.exceptions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace CitiDownloader.wrappers
{
    public class MailClient : IMailClient
    {
        public void SendEmail(string to, string from, string subject, string message)
        {
            SmtpClient client = new SmtpClient("mailhub.iastate.edu");

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(to);
                mailMessage.Body = message;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;

                int sendCount = 0;
                while (sendCount < 5)
                {
                    try
                    {
                        client.Send(mailMessage);
                        return;
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(3000);
                        sendCount++;
                    }
                }
            }

            throw new SendMailException("Unable to send email");
        }
    }
}
