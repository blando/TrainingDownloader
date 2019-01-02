using CitiDownloader.models;
using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public interface ICitiService
    {
        List<CitiRecord> GetRecords();
        void InsertSingleHistoryRecord(History history);
        string FindUser(CitiRecord citiRecord);
        IsuImportHistory InsertImportHistory(CitiRecord citiRecord);
        bool IsRecordVerified(CitiRecord citiRecord, out History history);
        string FindCourse(CitiRecord citiRecord);
        void UploadHistoryRecords(List<History> historyRecords);
    }
}
