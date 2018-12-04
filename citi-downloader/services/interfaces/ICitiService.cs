using CitiDownloader.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public interface ICitiService
    {
        List<CitiRecord> GetFullRecords();
        List<CitiRecord> GetIncrementalRecords();
    }
}
