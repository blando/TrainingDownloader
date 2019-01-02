using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.wrappers;
using static CitiDownloader.services.LogService;
using CitiDownloader.configurations;
using System.Linq;

namespace CitiDownloader.services
{
    public class TrainingService : ITrainingService
    {
        private ICitiService citiService {get; set;}
        private ILearnerWebServices learnerWebServices { get; set; }
        private ILogService logService { get; set; }
        private IReportingService reportingService { get; set; }
        private IMailService mailService { get; set; }
        private ApplicationConfiguration appConfig { get; set; }

        public TrainingService(ICitiService citiService, ILearnerWebServices learnerWebServices, ILogService logService, IReportingService reportingService, ApplicationConfiguration appConfig, IMailService mailService)
        {
            this.citiService = citiService;
            this.learnerWebServices = learnerWebServices;
            this.logService = logService;
            this.reportingService = reportingService;
            this.appConfig = appConfig;
            this.mailService = mailService;
        }

        public void ProcessRecords()
        {
            logService.LogMessage("Starting Training Downloader", EventType.Information);

            List<CitiRecord> citiRecords = citiService.GetRecords();
            if (citiRecords == null || citiRecords.Count == 0)
            {
                logService.LogMessage("No CitiRecords were returned", EventType.Warning);
                if (citiRecords == null)
                {
                    citiRecords = new List<CitiRecord>();
                }
            }
            else
            {
                logService.LogMessage(string.Format("Retrieved {0} records", citiRecords.Count), EventType.Information);
            }

            List<History> histories = new List<History>();
            foreach (CitiRecord citiRecord in citiRecords)
            {
                logService.FlushCache();
                logService.LogMessage(string.Format("Processing record for {0}", citiRecord.CitiId), EventType.Debug);

                // If the Import History record failed to import we'll stop processing this record
                IsuImportHistory isuImportHistory = citiService.InsertImportHistory(citiRecord);
                if (isuImportHistory.Id == 0)
                {
                    continue;
                }

                logService.LogMessage("Inserted import history record", EventType.Debug);

                // Get History record
                History history = learnerWebServices.GetHistoryByCurriculaId(citiRecord);
                if (history != null)
                {
                    logService.LogMessage("Found existing history record", EventType.Debug);

                    if (appConfig.processType == ApplicationConfiguration.ProcessType.Upload)
                    {
                        histories.Add(history);
                    }

                    continue;
                }

                // If record is verified we'll go to the next record
                if (citiService.IsRecordVerified(citiRecord, out history))
                {
                    if (appConfig.processType == ApplicationConfiguration.ProcessType.Upload)
                    {
                        histories.Add(history);
                    }
                    logService.LogMessage("Record is verified", EventType.Debug);
                    continue;
                }

                // Find the user
                citiRecord.UnivId = citiService.FindUser(citiRecord);
                if (citiRecord.UnivId == null)
                {
                    continue;
                }

                // Find the course
                citiRecord.CourseId = citiService.FindCourse(citiRecord);
                if (citiRecord.CourseId == null)
                {
                    continue;
                }

                // Use user and course to find if this records exists
                history = learnerWebServices.GetHistoryByCitiIdCourseIdDate(citiRecord);

                // Else we need to create a new record
                if (history == null)
                {
                    // Add the History record to the list of records to be returned
                    try
                    {
                        history = learnerWebServices.CreateHistoryRecord(citiRecord);
                        histories.Add(history);
                        citiService.InsertSingleHistoryRecord(history);
                        learnerWebServices.SetInsertedForImportRecord(Convert.ToInt32(isuImportHistory.Id));
                    }
                    catch (Exception exception)
                    {
                        string message = string.Format("An unknown error occurred while creating History record: {0}", exception.Message);
                        logService.LogMessage(message, EventType.Error);
                        reportingService.ReportSystemError(new SystemError { attachedObject = citiRecord, objectType = typeof(CitiRecord), message = message }, logService.GetCacheAndFlush());
                    }
                }
                else
                {
                    if (appConfig.processType == ApplicationConfiguration.ProcessType.Upload)
                    {
                        histories.Add(history);
                    }
                }
            }

            if (histories.Any())
            {
                if (appConfig.processType == ApplicationConfiguration.ProcessType.Upload)
                {
                    citiService.UploadHistoryRecords(histories);
                }
            }

            // If we have any errors to report send them
            try
            {
                mailService.SendMessages();
            }
            catch (SendMailException sendMailException)
            {
                logService.LogMessage(string.Format("Mail server timed out trying to send email: {0}", sendMailException.Message), LogService.EventType.Error);
            }

        }

        

    }
}
