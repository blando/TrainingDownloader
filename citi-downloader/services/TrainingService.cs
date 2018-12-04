using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using CitiDownloader.models;
using CitiDownloader.models.entities;
using CitiDownloader.wrappers;
using static CitiDownloader.models.ProcessTypeEnum;

namespace CitiDownloader.services
{
    public class TrainingService : ITrainingService
    {
        private ICitiService citiService;
        private ILearnerWebServices learnerWebServices;
        private ICsvWrapper csvWrapper;
        private ISftpWrapper sftpWrapper;

        public TrainingService(ICitiService citiService, ILearnerWebServices learnerWebServices, ICsvWrapper csvWrapper, ISftpWrapper sftpWrapper)
        {
            this.citiService = citiService;
            this.learnerWebServices = learnerWebServices;
            this.csvWrapper = csvWrapper;
            this.sftpWrapper = sftpWrapper;
        }

        public void InsertHistoryRecords(List<History> historyRecords)
        {
            foreach(History history in historyRecords)
            {
                learnerWebServices.InsertHistory(history);
            }
        }

        public List<History> ProcessRecords(ProcessType processType)
        {
            List<CitiRecord> citiRecords;
            if (processType == ProcessType.Full)
            {
                citiRecords = citiService.GetFullRecords();
            }
            else
            {
                citiRecords = citiService.GetIncrementalRecords();
            }

            List<History> histories = new List<History>();
            foreach(CitiRecord citiRecord in citiRecords)
            {
                learnerWebServices.InsertImportHistory(citiRecord);
                
                // If this record is already verified we'll go on to the next
                if (learnerWebServices.IsVerified(citiRecord))
                {
                    History history = learnerWebServices.GetHistoryByCurriculaId(citiRecord);
                    if (history != null)
                    {
                        histories.Add(history);
                    }  
                    continue;
                }
                try
                {
                    citiRecord.UnivId = learnerWebServices.FindUser(citiRecord);
                }
                catch(InvalidUserException)
                {
                    continue;
                }
                catch(UnknownUserException)
                {
                    // TODO: Report unknown user
                }
                citiRecord.CourseId = learnerWebServices.FindCourseId(citiRecord);
                
                histories.Add(learnerWebServices.CreateHistoryRecord(citiRecord));
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
