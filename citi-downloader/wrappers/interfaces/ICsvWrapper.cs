using CitiDownloader.models;
using CitiDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public interface ICsvWrapper
    {
        List<CitiRecord> GetCitiRecords(string file);
        string WriteHistoryRecordsToFile(List<History> histories);
    }
}
