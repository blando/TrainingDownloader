using CitiDownloader.wrappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.configurations;

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

        public string DownloadFile()
        {
            webClientWrapper.DownloadFile(config.DownloadUrl, config.SaveFilePath);
            return config.SaveFilePath;
        }
    }
}
