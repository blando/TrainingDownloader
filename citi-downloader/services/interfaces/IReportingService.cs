using TrainingDownloader.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.services
{
    public interface IReportingService
    {
        void ReportUnknownUser(VendorRecord vRecord, List<string> messages);
        void ReportUnknownCourse(VendorRecord vRecord, List<string> messages);
        void ReportSystemError(SystemError systemError, List<string> messages);
        List<ReportMessage> GetUnknownUsers();
        List<ReportMessage> GetUnknownCourses();
        List<ReportMessage> GetSystemErrors();
        bool HasErrors();
    }
}
