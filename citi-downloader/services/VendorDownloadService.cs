using TrainingDownloader.wrappers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingDownloader.configurations;

namespace TrainingDownloader.services
{
    public class VendorDownloadService : IVendorDownloadService
    {
        private ApplicationConfiguration config { get; set; }
        private IWebClientWrapper webClientWrapper { get; set; }

        public VendorDownloadService(ApplicationConfiguration configuration, IWebClientWrapper webClientWrapper)
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
