using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.wrappers;
using static CitiDownloader.models.ProcessTypeEnum;
using ISULogger;
using static CitiDownloader.services.Log;

namespace CitiDownloader.services
{
    public class TrainingService : ITrainingService
    {
        private ICitiService citiService;
        private ILearnerWebServices learnerWebServices;
        private ICsvWrapper csvWrapper;
        private ISftpWrapper sftpWrapper;
        private ILog log;
        private IReportingService reportingService;

        public TrainingService(ICitiService citiService, ILearnerWebServices learnerWebServices, ICsvWrapper csvWrapper, ISftpWrapper sftpWrapper, ILog log, IReportingService reportingService)
        {
            this.citiService = citiService;
            this.learnerWebServices = learnerWebServices;
            this.csvWrapper = csvWrapper;
            this.sftpWrapper = sftpWrapper;
            this.log = log;
            this.reportingService = reportingService;
        }

        public void InsertHistoryRecords(List<History> historyRecords)
        {
            foreach(History history in historyRecords)
            {
                bool inserted;
                learnerWebServices.InsertHistory(history, out inserted);
                if (inserted)
                {
                    log.LogMessage(
                        string.Format("Training inserted for Learner {0} and Course {1} with date {2}", history.LearnerId, history.CourseId, history.StatusDate), 
                        EventType.Information);
                }
            }
        }

        private List<CitiRecord> GetCitiRecords(ProcessType processType)
        {
            try
            {
                if (processType == ProcessType.Full)
                {
                    log.LogMessage("Getting full CITI data", EventType.Information);
                    return citiService.GetFullRecords();
                }
                else
                {
                    log.LogMessage("Getting incremental CITI data", EventType.Information);
                    return citiService.GetIncrementalRecords();
                }
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred retrieving records: {0}", exception.Message);
                log.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = null, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                return null;
            }
        }

        private bool InsertImportHistory(CitiRecord citiRecord)
        {
            try
            {
                learnerWebServices.InsertImportHistory(citiRecord);
                return true;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while inserting Import History: {0}", exception.Message);
                log.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                return false;
            }
        }

        private bool IsRecordVerified(CitiRecord citiRecord, ref List<History> histories)
        {
            try
            {
                // If this record is already verified we'll go on to the next
                if (learnerWebServices.IsVerified(citiRecord))
                {
                    log.LogMessage("Record is verified", EventType.Debug);
                    History history = learnerWebServices.GetHistoryByCurriculaId(citiRecord);
                    if (history != null)
                    {
                        histories.Add(history);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving History record: {0}", exception.Message);
                log.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                return true;
            }
        }

        private string FindUser(CitiRecord citiRecord)
        {
            try
            {
                string univId = learnerWebServices.FindUser(citiRecord);
                log.LogMessage(string.Format("Found user with id {0}", citiRecord.UnivId), EventType.Debug);
                return univId;
            }
            catch (InvalidUserException)
            {
                log.LogMessage("User is marked as invalid", EventType.Debug);
                return null;
            }
            catch (UnknownUserException)
            {
                log.LogMessage(string.Format("Unknown user with CitiId {0}", citiRecord.CitiId), EventType.Warning);
                reportingService.ReportUnknownUser(citiRecord, log.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving User Id: {0}", exception.Message);
                log.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                return null;
            }
        }

        private string FindCourse(CitiRecord citiRecord)
        {
            try
            {
                string courseId = learnerWebServices.FindCourseId(citiRecord);
                log.LogMessage(string.Format("Found course id {0}", citiRecord.CourseId), EventType.Debug);
                return courseId;
            }
            catch (UnknownCourseException)
            {
                log.LogMessage(string.Format("Unknown course {0}: {1}", citiRecord.GroupId, citiRecord.Group), EventType.Warning);
                reportingService.ReportUnknownCourse(citiRecord, log.GetCacheAndFlush());
                return null;
            }
            catch (Exception exception)
            {
                string message = string.Format("An unknown error occurred while retrieving course Id: {0}", exception.Message);
                log.LogMessage(message, EventType.Error);
                reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                return null;
            }
        }

        public List<History> ProcessRecords(ProcessType processType)
        {
            List<CitiRecord> citiRecords = GetCitiRecords(processType);
            if (citiRecords == null || citiRecords.Count == 0)
            {
                log.LogMessage("No CitiRecords were returned", EventType.Warning);
                return null;
            } 

            log.LogMessage(string.Format("Retrieved {0} records", citiRecords.Count), EventType.Information);

            List<History> histories = new List<History>();
            foreach(CitiRecord citiRecord in citiRecords)
            {
                log.LogMessage(string.Format("Processing record for {0}", citiRecord.CitiId), EventType.Debug);

                // If the Import History record failed to import we'll stop processing this record
                if (!InsertImportHistory(citiRecord))
                {
                    continue;
                }

                log.LogMessage("Inserted import history record", EventType.Debug);

                // If the record is already verified, then we don't need to do anymore processing
                if (IsRecordVerified(citiRecord, ref histories))
                {
                    continue;
                }

                log.LogMessage("History is not verified yet", EventType.Debug);

                // If a user was not found when we skip this record
                citiRecord.UnivId = FindUser(citiRecord);
                if (citiRecord.UnivId == null)
                {
                    continue;
                }

                // If a course was not found then we skip this record
                citiRecord.CourseId = FindCourse(citiRecord);
                if (citiRecord.CourseId == null)
                {
                    continue;
                }

                // Add the History record to the list of records to be returned
                try
                {
                    histories.Add(learnerWebServices.CreateHistoryRecord(citiRecord));
                }
                catch(Exception exception)
                {
                    string message = string.Format("An unknown error occurred while creating History record: {0}", exception.Message);
                    log.LogMessage(message, EventType.Error);
                    reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, log.GetCacheAndFlush());
                    continue;
                }

                log.FlushCache();
            }

            return histories;
        }


        public void UploadHistoryRecords(List<History> historyRecords)
        {
            string filePath = csvWrapper.WriteHistoryRecordsToFile(historyRecords);
            sftpWrapper.Upload(filePath);
        }

        
    }
}
