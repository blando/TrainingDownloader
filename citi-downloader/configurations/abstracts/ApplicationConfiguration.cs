using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitiDownloader.wrappers
{
    public abstract class ApplicationConfiguration
    {
        protected abstract IConfiguration configuration { get; set; }
        protected abstract DateTime dateTime { get; set; }
        public abstract string FullFile { get; }
        public abstract string IncrementalFile { get; }
        public abstract string SavePath { get; }
        public abstract string SftpServer { get; }
        public abstract string SftpUserName { get; }
        public abstract string SftpPassword { get; }
        public abstract string SftpRemotePath { get; }
        public abstract string SftpUploadFileName { get; }
        public abstract string OutputFileName { get; }
        public abstract string FullOutputPath { get; }
        public abstract string FullFileName {get; }
        public abstract string IncrementalFileName { get; }
        public abstract string FullSavePath { get; }
        public abstract string IncrementalSavePath { get; }
        public abstract string AdminUrl { get; }
        public abstract string AdminMailToAddress { get; }
        public abstract string AdminMailSubject { get; }
        public abstract string SysAdminMailToAddress { get; }
        public abstract string SysAdminMailSubject { get; }
        public abstract string MailSenderAddress { get; }
        public abstract string EventLogName { get; }

    }
}
