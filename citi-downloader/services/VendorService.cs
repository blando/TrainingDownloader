using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.exceptions;
using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using TrainingDownloader.wrappers;
using static TrainingDownloader.services.LogService;

namespace TrainingDownloader.services
{
    public class VendorService : IVendorService
    {
        private IVendorDownloadService vendorDownloadService;
        private ICsvClient csvClient;
        private ILearnerWebServices learnerWebServices;
        private ILogService logService;
        private IReportingService reportingService;
        private ISftpClient sftpClient;

        public VendorService(IVendorDownloadService vendorDownloadService, ICsvClient csvClient, ILearnerWebServices learnerWebServices, ILogService logService, IReportingService reportingService, ISftpClient sftpClient)
        {
            this.vendorDownloadService = vendorDownloadService;
            this.csvClient = csvClient;
            this.learnerWebServices = learnerWebServices;
            this.logService = logService;
            this.reportingService = reportingService;
            this.sftpClient = sftpClient;
        }

        public List<VendorRecord> GetRecords()
        {
            logService.LogMessage("Getting Vendor data", EventType.Information);
            try
            {
                string file = vendorDownloadService.DownloadFile();
                return csvClient.GetVendorRecords(file);
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred retrieving records: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = null, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
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


        public string FindUser(VendorRecord vRecord)
        {
            try
            {
                string univId = learnerWebServices.FindUser(vRecord);
                logService.LogMessage(string.Format("Found user with id {0}", vRecord.UnivId), EventType.Debug);
                return univId;
            }
            catch (InvalidUserException)
            {
                logService.LogMessage("User is marked as invalid", EventType.Debug);
                return null;
            }
            catch (UnknownUserException)
            {
                logService.LogMessage(string.Format("Unknown user with Vendor Id {0}", vRecord.VendorUserId), EventType.Warning);
                reportingService.ReportUnknownUser(vRecord, logService.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving User Id: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = vRecord, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public IsuImportHistory InsertImportHistory(VendorRecord vRecord)
        {
            try
            {
                IsuImportHistory isuImportHistory = learnerWebServices.InsertImportHistory(vRecord);
                return isuImportHistory;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while inserting Import History: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = vRecord, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public bool IsRecordVerified(VendorRecord vRecord, out History history)
        {
            history = null;
            try
            {
                // If this record is already verified we'll go on to the next
                if (learnerWebServices.IsVerified(vRecord))
                {
                    logService.LogMessage("Record is verified", EventType.Debug);
                    history = learnerWebServices.GetHistoryByCurriculaId(vRecord);
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving History record: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = vRecord, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
                return false;
            }
        }

        public string FindCourse(VendorRecord vRecord)
        {
            try
            {
                string courseId = learnerWebServices.FindCourseId(vRecord);
                logService.LogMessage(string.Format("Found course id {0}", vRecord.LearnerWebCourseId), EventType.Debug);
                return courseId;
            }
            catch (UnknownCourseException)
            {
                logService.LogMessage(string.Format("Unknown course {0}: {1}", vRecord.VendorCourseId, vRecord.VendorCourseName), EventType.Warning);
                reportingService.ReportUnknownCourse(vRecord, logService.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving course Id: {0}", exception.Message);
                logService.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = vRecord, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
                return null;
            }
        }

        public void UploadHistoryRecords(List<History> historyRecords)
        {
            string filePath = csvClient.WriteHistoryRecordsToFile(historyRecords);
            sftpClient.Upload(filePath);
        }

    }
}
