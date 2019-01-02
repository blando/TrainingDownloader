using CitiDownloader.models;
using CitiDownloader.wrappers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HtmlTags;
using CitiDownloader.configurations;

namespace CitiDownloader.services
{
    public class MailService : IMailService
    {
        private IReportingService reportingService;
        private IMailClient mailWrapper;
        private ApplicationConfiguration config;

        public MailService(IReportingService reportingService, IMailClient mailWrapper, ApplicationConfiguration config)
        {
            this.reportingService = reportingService;
            this.mailWrapper = mailWrapper;
            this.config = config;
        }

        public void SendMessages()
        {
            if (reportingService.HasErrors())
            {
                List<ReportMessage> users = reportingService.GetUnknownUsers();
                List<ReportMessage> courses = reportingService.GetUnknownCourses();

                if (users.Any() || courses.Any())
                {
                    
                    StringBuilder body = new StringBuilder();
                    body.Append("<h2>CITI Training Messages</h2>");
                    if (courses.Any())
                    {
                        TableTag table = new TableTag();
                        table.AddHeaderRow(CitiRecord.GetTableHeaderRow());

                        body.Append("<p><h4>Missing Courses</h4>The following courses are unknown and need to be setup in LearnerWeb and tied to the Citi-Course-Id.</p>");

                        foreach (ReportMessage reportMessage in courses)
                        {
                            CitiRecord citiRecord = reportMessage.attachedObject as CitiRecord;
                            table.AddBodyRow(citiRecord.ToTableRow());
                        }
                        body.Append(table.ToHtmlString());
                    }

                    if (users.Any())
                    {
                        body.Append("<p><h4>Missing Users</h4>The following users are unknown and need to be setup in LearnerWeb and tied to the Citi-Id.</p>");
           
                        TableTag table = new TableTag();
                        table.AddHeaderRow(CitiRecord.GetTableHeaderRow());

                        foreach (ReportMessage reportMessage in courses)
                        {
                            CitiRecord citiRecord = reportMessage.attachedObject as CitiRecord;
                            table.AddBodyRow(citiRecord.ToTableRow());
                        }
                        body.Append(table.ToHtmlString());
                    }

                    mailWrapper.SendEmail(config.AdminMailToAddress, config.MailSenderAddress, config.AdminMailSubject, body.ToString());
                    
                }

                List<ReportMessage> systemErrors = reportingService.GetSystemErrors();

                if (systemErrors.Any())
                {
                    StringBuilder body = new StringBuilder();
                    body.Append("<p><h4>System Error Messages</h4></p>");
                    
                    foreach (ReportMessage reportMessage in systemErrors)
                    {
                        SystemError systemError = reportMessage.attachedObject as SystemError;
                        body.Append("<p><b>" + systemError.message + "</b><br/><br/>");
                        foreach(string message in reportMessage.messages)
                        {
                            body.Append(message);
                        }
                        body.Append("</p>");
                    }

                    mailWrapper.SendEmail(config.SysAdminMailToAddress, config.MailSenderAddress, config.SysAdminMailSubject, body.ToString());
                }
            }
        }

    }
}
