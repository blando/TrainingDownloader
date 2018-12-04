using CitiDownloader.models;
using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CitiDownloader.models.ProcessTypeEnum;

namespace CitiDownloader.services
{
    public interface ITrainingService
    {
        List<History> ProcessRecords(ProcessType processType);
        void UploadHistoryRecords(List<History> historyRecords);
        void InsertHistoryRecords(List<History> historyRecords);
    }
}
