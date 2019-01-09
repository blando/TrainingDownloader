using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.services
{
    public interface ILearnerWebServices
    {
        IsuImportHistory InsertImportHistory(VendorRecord vRecord);
        bool IsValid(VendorRecord vRecord);
        string FindUser(VendorRecord vRecord);
        IsuImportHistory SetInsertedForImportRecord(int id);
        string FindCourseId(VendorRecord VendorRecord);
        int InsertHistory(History history, out bool inserted);
        History CreateHistoryRecord(VendorRecord vRecord);
        bool IsVerified(VendorRecord vRecord);
        History GetHistoryByCurriculaId(VendorRecord vRecord);
        History GetHistoryByVendorIdCourseIdDate(VendorRecord vRecord);
        void UpdateImportHistoryWithCurriculaId(VendorRecord vRecord, History history);
    }
}
