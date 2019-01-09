using TrainingDownloader.exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingDownloader.models;
using System.Configuration;
using static TrainingDownloader.configurations.CommandLineConfiguration;

namespace TrainingDownloader.configurations
{
    public class ApplicationConfiguration
    {
        
        public DateTime dateTime { get; set; }
        public string dateTimeString
        {
            get
            {
                return this.dateTime.ToString("yyyyMMddHHmmss");
            }
        }

        // Automapped from configuration properties

        public string FullFileDownloadUrl { get; set; }
        public string IncrementalFileDownloadUrl { get; set; }
        public string LocalSavePath { get; set; }
        public string OutputFileName { get; set; }
        public string FullFileName { get; set; }
        public string IncrementalFileName { get; set; }
        public string SftpServer { get; set; }
        public string SftpUserName { get; set; }
        public string SftpPassword { get; set; }
        public string SftpRemotePath { get; set; }
        public string SftpUploadFileName { get; set; }
        public string AdminUrl { get; set; }
        public string AdminMailToAddress { get; set; }
        public string AdminMailSubject { get; set; }
        public string SysAdminMailToAddress { get; set; }
        public string SysAdminMailSubject { get; set; }
        public string MailSenderAddress { get; set; }
        public string EventLogName { get; set; }
        public string LinuxTextLogName { get; set; }
        public string VendorApplication { get; set; }
        public FieldMap FieldMaps { get; set; }

        // Download & Save Settings generated
        public string SaveFileName => string.Format((importType == ImportType.Full ? FullFileName : IncrementalFileName), dateTimeString);
        public string OutputFileNameFormatted => string.Format(OutputFileName, dateTimeString);
        public string DownloadUrl => this.importType == ImportType.Full ? FullFileDownloadUrl : IncrementalFileDownloadUrl;
        public string SaveFilePath => string.Format("{0}\\{1}", LocalSavePath, SaveFileName);
        public string OutputFilePath => string.Format("{0}\\{1}", LocalSavePath, OutputFileNameFormatted);

        // Sftp Settings generated
        public string SftpUploadFileNameStr => string.Format(this.SftpUploadFileName, dateTime.ToString("yyyyMMddHHmmss"));

        // Enums
        public ProcessType processType { get; set; }
        public ImportType importType { get; set; }

        // Process Settings
        public bool verbose { get; set; }
        public ApplicationType applicationType{ get; set; }


    }
}
