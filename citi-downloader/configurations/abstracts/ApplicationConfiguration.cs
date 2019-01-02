using CitiDownloader.exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CitiDownloader.configurations
{
    public abstract class ApplicationConfiguration
    {
        protected abstract IConfiguration configuration { get; set; }
        public abstract DateTime dateTime { get; set; }
        public abstract string dateTimeString { get; }

        // Download & Save Settings
        public abstract string DownloadUrl { get; }
        public abstract string SaveFilePath { get; }    
        public abstract string OutputFilePath { get; }

        // SFTP Server Settings
        public abstract string SftpServer { get; }
        public abstract string SftpUserName { get; }
        public abstract string SftpPassword { get; }
        public abstract string SftpRemotePath { get; }
        public abstract string SftpUploadFileName { get; }

        // Email Settings
        public abstract string AdminUrl { get; }
        public abstract string AdminMailToAddress { get; }
        public abstract string AdminMailSubject { get; }
        public abstract string SysAdminMailToAddress { get; }
        public abstract string SysAdminMailSubject { get; }
        public abstract string MailSenderAddress { get; }

        // Log Settings
        public abstract string EventLogName { get; }
        public abstract string LinuxTextLogName { get; }

        // Enums
        public enum ImportType { Full = 1, Incremental = 2 };
        public enum ProcessType { Upload = 1, Insert = 2 };
        public abstract ProcessType processType { get; set; }
        public abstract ImportType importType { get; set; }

        // Process Settings
        public abstract bool verbose { get; set; }

        protected ApplicationConfiguration(string[] args)
        {
            List<string> arguments = new List<string>(args);

            string settingsFile = arguments.Where(s => s.Contains(".json")).FirstOrDefault();

            if (settingsFile == null)
            {
                throw new ParameterException("Missing JSON configuration file parameter");
            }

            if (arguments.Contains("full"))
            {
                importType = ImportType.Full;
            }
            else if (arguments.Contains("delta"))
            {
                importType = ImportType.Incremental;
            }
            else
            {
                throw new ParameterException("Missing import parameter (full/delta)");
            }

            if (arguments.Contains("upload"))
            {
                processType = ProcessType.Upload;
            }
            else if (arguments.Contains("insert"))
            {
                processType = ProcessType.Insert;
            }
            else
            {
                throw new ParameterException("Missing process type parameter (upload/insert");
            }

            if (arguments.Contains("verbose"))
            {
                this.verbose = true;
            }

            this.configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
                .Build();
            dateTime = DateTime.Now;
        }


    }
}
