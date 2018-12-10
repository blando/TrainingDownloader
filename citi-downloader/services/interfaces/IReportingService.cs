using CitiDownloader.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public interface IReportingService
    {
        void ReportUnknownUser(CitiRecord citiRecord, List<string> messages);
        void ReportUnknownCourse(CitiRecord citiRecord, List<string> messages);
        void ReportSystemError(SystemError systemError, List<string> messages);
        List<ReportMessage> GetUnknownUsers();
        List<ReportMessage> GetUnknownCourses();
        List<ReportMessage> GetSystemErrors();
        bool HasErrors();
    }
}
