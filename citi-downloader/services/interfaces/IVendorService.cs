using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.services
{
    public interface IVendorService
    {
        List<VendorRecord> GetRecords();
        void InsertSingleHistoryRecord(History history);
        string FindUser(VendorRecord vRecord);
        IsuImportHistory InsertImportHistory(VendorRecord vRecord);
        bool IsRecordVerified(VendorRecord vRecord, out History history);
        string FindCourse(VendorRecord vRecord);
        void UploadHistoryRecords(List<History> historyRecords);
    }
}
