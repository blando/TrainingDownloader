using CitiDownloader.models;
using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public interface ILearnerWebServices
    {
        IsuImportHistory InsertImportHistory(CitiRecord citiRecord);
        bool IsValid(CitiRecord citiRecord);
        string FindUser(CitiRecord citiRecord);
        string FindCourseId(CitiRecord citiRecord);
        void InsertHistory(History history);
        History CreateHistoryRecord(CitiRecord citiRecord);
        bool IsVerified(CitiRecord citi);
        History GetHistoryByCurriculaId(CitiRecord citiRecord);
        
    }
}
