using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.models;
using CitiDownloader.wrappers;

namespace CitiDownloader.services
{
    public class CitiService : ICitiService
    {
        private ICitiDownloadService citiDownloadService;
        private ICsvWrapper csvWrapper;

        public CitiService(ICitiDownloadService citiDownloadService, ICsvWrapper csvWrapper)
        {
            this.citiDownloadService = citiDownloadService;
            this.csvWrapper = csvWrapper;
        }
        public List<CitiRecord> GetFullRecords()
        {
            string file = citiDownloadService.DownloadFullFile();
            return csvWrapper.GetCitiRecords(file);
        }

        public List<CitiRecord> GetIncrementalRecords()
        {
            string file = citiDownloadService.DownloadIncrementalFile();
            return csvWrapper.GetCitiRecords(file);
        }
    }
}
