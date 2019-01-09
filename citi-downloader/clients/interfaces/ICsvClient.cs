using TrainingDownloader.models;
using TrainingDownloader.models.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDownloader.wrappers
{
    public interface ICsvClient
    {
        List<VendorRecord> GetVendorRecords(string file);
        string WriteHistoryRecordsToFile(List<History> histories);
    }
}
