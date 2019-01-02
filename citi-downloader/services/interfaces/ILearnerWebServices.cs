﻿using CitiDownloader.models;
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
        IsuImportHistory SetInsertedForImportRecord(int id);
        string FindCourseId(CitiRecord citiRecord);
        int InsertHistory(History history, out bool inserted);
        History CreateHistoryRecord(CitiRecord citiRecord);
        bool IsVerified(CitiRecord citi);
        History GetHistoryByCurriculaId(CitiRecord citiRecord);
        History GetHistoryByCitiIdCourseIdDate(CitiRecord citiRecord);
        void UpdateImportHistoryWithCurriculaId(CitiRecord citiRecord, History history);
    }
}
