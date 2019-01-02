using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.wrappers;
using static CitiDownloader.services.LogService;

namespace CitiDownloader.services
{
    public class CitiService : ICitiService
    {
        private ICitiDownloadService citiDownloadService;
        private ICsvClient csvWrapper;
        private ILearnerWebServices learnerWebServices;
        private ILogService logService;
        private IReportingService reportingService;
        private ISftpClient sftpClient;

        public CitiService(ICitiDownloadService citiDownloadService, ICsvClient csvWrapper, ILearnerWebServices learnerWebServices, ILogService logService, IReportingService reportingService, ISftpClient sftpClient)
        {
            this.citiDownloadService = citiDownloadService;
            this.csvWrapper = csvWrapper;
            this.learnerWebServices = learnerWebServices;
            this.logService = logService;
            this.reportingService = reportingService;
            this.sftpClient = sftpClient;
        }

        public List<CitiRecord> GetRecords()
        {
            logService.LogMessage("Getting CITI data", EventType.Information);
            try
            {
                string file = citiDownloadService.DownloadFile();
                return csvWrapper.GetCitiRecords(file);
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred retrieving records: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = null, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public void InsertSingleHistoryRecord(History history)
        {
            int curriculaId = learnerWebServices.InsertHistory(history, out bool inserted);
            if (inserted)
            {
                logService.LogMessage(
                    string.Format("Training inserted for Learner {0} and Course {1} with date {2}", history.LearnerId, history.CourseId, history.StatusDate),
                    EventType.Information);
            }
        }


        public string FindUser(CitiRecord citiRecord)
        {
            try
            {
                string univId = learnerWebServices.FindUser(citiRecord);
                logService.LogMessage(string.Format("Found user with id {0}", citiRecord.UnivId), EventType.Debug);
                return univId;
            }
            catch (InvalidUserException)
            {
                logService.LogMessage("User is marked as invalid", EventType.Debug);
                return null;
            }
            catch (UnknownUserException)
            {
                logService.LogMessage(string.Format("Unknown user with CitiId {0}", citiRecord.CitiId), EventType.Warning);
                reportingService.ReportUnknownUser(citiRecord, logService.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving User Id: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public IsuImportHistory InsertImportHistory(CitiRecord citiRecord)
        {
            try
            {
                IsuImportHistory isuImportHistory = learnerWebServices.InsertImportHistory(citiRecord);
                return isuImportHistory;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while inserting Import History: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public bool IsRecordVerified(CitiRecord citiRecord, out History history)
        {
            history = null;
            try
            {
                // If this record is already verified we'll go on to the next
                if (learnerWebServices.IsVerified(citiRecord))
                {
                    logService.LogMessage("Record is verified", EventType.Debug);
                    history = learnerWebServices.GetHistoryByCurriculaId(citiRecord);
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving History record: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                return false;
            }
        }

        public string FindCourse(CitiRecord citiRecord)
        {
            try
            {
                string courseId = learnerWebServices.FindCourseId(citiRecord);
                logService.LogMessage(string.Format("Found course id {0}", citiRecord.CourseId), EventType.Debug);
                return courseId;
            }
            catch (UnknownCourseException)
            {
                logService.LogMessage(string.Format("Unknown course {0}: {1}", citiRecord.CitiCourseId, citiRecord.Group), EventType.Warning);
                reportingService.ReportUnknownCourse(citiRecord, logService.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving course Id: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public void UploadHistoryRecords(List<History> historyRecords)
        {
            string filePath = csvWrapper.WriteHistoryRecordsToFile(historyRecords);
            sftpClient.Upload(filePath);
        }
    }
}
