using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrainingDownloader.models;

namespace TrainingDownloader.services
{
    public class ReportingService : IReportingService
    {
        private List<ReportMessage> UnknownUserMessages;
        private List<ReportMessage> UnknownCourseMessages;
        private List<ReportMessage> SystemErrors;

        public ReportingService()
        {
            this.UnknownUserMessages = new List<ReportMessage>();
            this.UnknownCourseMessages = new List<ReportMessage>();
            this.SystemErrors = new List<ReportMessage>();
        }

        public List<ReportMessage> GetSystemErrors()
        {
            return SystemErrors;
        }

        public List<ReportMessage> GetUnknownCourses()
        {
            return UnknownCourseMessages;
        }

        public List<ReportMessage> GetUnknownUsers()
        {
            return UnknownUserMessages;
        }

        public bool HasErrors()
        {
            if (UnknownUserMessages.Any() || UnknownCourseMessages.Any() || SystemErrors.Any())
            {
                return true;
            }
            return false;
        }

        public void ReportSystemError(SystemError systemError, List<string> messages)
        {
            SystemErrors.Add(new ReportMessage
            {
                attachedObject = systemError,
                messages = messages
            });

        }

        public void ReportUnknownCourse(VendorRecord vRecord, List<string> messages)
        {
            UnknownCourseMessages.Add(new ReportMessage
            {
                attachedObject = vRecord,
                messages = messages
            });
        }

        public void ReportUnknownUser(VendorRecord vRecord, List<string> messages)
        {
            UnknownUserMessages.Add(new ReportMessage
            {
                attachedObject = vRecord,
                messages = messages
            });
        }
    }
}
