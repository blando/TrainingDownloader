using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.exceptions;
using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using TrainingDownloader.wrappers;
using static TrainingDownloader.services.LogService;
using TrainingDownloader.configurations;
using System.Linq;

namespace TrainingDownloader.services
{
    public class TrainingService : ITrainingService
    {
        private IVendorService vendorService {get; set;}
        private ILearnerWebServices learnerWebServices { get; set; }
        private ILogService logService { get; set; }
        private IReportingService reportingService { get; set; }
        private IMailService mailService { get; set; }
        private ApplicationConfiguration appConfig { get; set; }

        public TrainingService(IVendorService vendorService, ILearnerWebServices learnerWebServices, ILogService logService, IReportingService reportingService, ApplicationConfiguration appConfig, IMailService mailService)
        {
            this.vendorService = vendorService;
            this.learnerWebServices = learnerWebServices;
            this.logService = logService;
            this.reportingService = reportingService;
            this.appConfig = appConfig;
            this.mailService = mailService;
        }

        public void ProcessRecords()
        {
            logService.LogMessage("Starting Training Downloader", EventType.Information);

            List<VendorRecord> vRecords = vendorService.GetRecords();
            if (vRecords == null || vRecords.Count == 0)
            {
                logService.LogMessage("No Records were returned", EventType.Warning);
                if (vRecords == null)
                {
                    vRecords = new List<VendorRecord>();
                }
            }
            else
            {
                logService.LogMessage(string.Format("Retrieved {0} records", vRecords.Count), EventType.Information);
            }

            List<History> histories = new List<History>();
            foreach (VendorRecord vRecord in vRecords)
            {
                logService.FlushCache();
                logService.LogMessage(string.Format("Processing record for {0}", vRecord.VendorUserId), EventType.Debug);

                // If the Import History record failed to import we'll stop processing this record
                IsuImportHistory isuImportHistory = vendorService.InsertImportHistory(vRecord);
                if (isuImportHistory.Id == 0)
                {
                    continue;
                }

                logService.LogMessage("Inserted import history record", EventType.Debug);

                if (!vRecord.IsComplete)
                {
                    logService.LogMessage("Training record is not complete", EventType.Debug);
                    continue;
                }

                // Get History record
                History history = learnerWebServices.GetHistoryByCurriculaId(vRecord);
                if (history != null)
                {
                    logService.LogMessage("Found existing history record", EventType.Debug);

                    if (appConfig.processType == CommandLineConfiguration.ProcessType.Upload)
                    {
                        histories.Add(history);
                    }

                    continue;
                }

                // If record is verified we'll go to the next record
                if (vendorService.IsRecordVerified(vRecord, out history))
                {
                    if (appConfig.processType == CommandLineConfiguration.ProcessType.Upload && history != null)
                    {
                        histories.Add(history);
                    }
                    logService.LogMessage("Record is verified", EventType.Debug);
                    continue;
                }

                // Find the user
                vRecord.UnivId = vendorService.FindUser(vRecord);
                if (vRecord.UnivId == null)
                {
                    continue;
                }

                // Find the course
                vRecord.LearnerWebCourseId = vendorService.FindCourse(vRecord);
                if (vRecord.LearnerWebCourseId == null)
                {
                    continue;
                }

                // Use user and course to find if this records exists
                history = learnerWebServices.GetHistoryByVendorIdCourseIdDate(vRecord);

                // Else we need to create a new record
                if (history == null)
                {
                    // Add the History record to the list of records to be returned
                    try
                    {
                        history = learnerWebServices.CreateHistoryRecord(vRecord);
                        histories.Add(history);
                        vendorService.InsertSingleHistoryRecord(history);
                        learnerWebServices.SetInsertedForImportRecord(Convert.ToInt32(isuImportHistory.Id));
                    }
                    catch (Exception exception)
                    {
                        string message = string.Format("An unknown error occurred while creating History record: {0}", exception.Message);
                        logService.LogMessage(message, EventType.Error);
                        reportingService.ReportSystemError(new SystemError { attachedObject = vRecord, objectType = typeof(VendorRecord), message = message }, logService.GetCacheAndFlush());
                    }
                }
                else
                {
                    if (appConfig.processType == CommandLineConfiguration.ProcessType.Upload)
                    {
                        histories.Add(history);
                    }
                }
            }

            if (histories.Any())
            {
                if (appConfig.processType == CommandLineConfiguration.ProcessType.Upload)
                {
                    vendorService.UploadHistoryRecords(histories);
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
