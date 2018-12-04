using CitiDownloader.wrappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.services
{
    public class CitiDownloadService : ICitiDownloadService
    {
        private ApplicationConfiguration config { get; set; }
        private IWebClientWrapper webClientWrapper { get; set; }

        public CitiDownloadService(ApplicationConfiguration configuration, IWebClientWrapper webClientWrapper)
        {
            this.config = configuration;
            this.webClientWrapper = webClientWrapper;
        }

        public string DownloadFullFile()
        {
            webClientWrapper.DownloadFile(config.FullFile, config.FullSavePath);
            return config.FullSavePath;
        }

        public string DownloadIncrementalFile()
        {
            webClientWrapper.DownloadFile(config.IncrementalFile, config.IncrementalSavePath);
            return config.IncrementalSavePath;
        }
    }
}
