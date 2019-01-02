using System;
using System.Collections.Generic;
using System.Text;
using CitiDownloader.exceptions;
using Microsoft.Extensions.Configuration;

namespace CitiDownloader.configurations
{
    public class AppConfig : ApplicationConfiguration
    {
        protected override IConfiguration configuration { get; set; }
        private DateTime _dateTime { get; set; }
        public override DateTime dateTime
        {
            get => this._dateTime;
            set => this._dateTime = value;
        }
        public override string dateTimeString => this._dateTime.ToString("yyyyMMddHHmmss");

        // Download & Save Settings from settings file
        private string FullFileDownloadUrl => this.configuration["FullFileDownloadUrl"];
        private string IncrementalFileDownloadUrl => this.configuration["IncrementalFileDownloadUrl"];
        private string LocalSavePath => this.configuration["LocalSavePath"];
        private string OutputFileName => string.Format(this.configuration["OutputFileName"], dateTime.ToString("yyyyMMddHHmmss"));
        private string FullFileName => string.Format(this.configuration["FullFileName"], dateTime.ToString("yyyyMMddHHmmss"));
        private string IncrementalFileName => string.Format(this.configuration["IncrementalFileName"], dateTime.ToString("yyyyMMddHHmmss"));
        
        // Download & Save Settings generated
        public override string DownloadUrl => this._importType == ImportType.Full ? FullFileDownloadUrl : IncrementalFileDownloadUrl;
        public override string SaveFilePath => string.Format("{0}\\{1}", LocalSavePath, this._importType == ImportType.Full ? FullFileName : IncrementalFileName);
        public override string OutputFilePath => string.Format("{0}\\{1}", LocalSavePath, OutputFileName);

        // Email Settings from file
        public override string AdminUrl => this.configuration["AdminUrl"];
        public override string AdminMailToAddress => this.configuration["AdminMailToAddress"];
        public override string AdminMailSubject => this.configuration["AdminMailSubject"];
        public override string SysAdminMailToAddress => this.configuration["SysAdminMailToAddress"];
        public override string SysAdminMailSubject => this.configuration["SysAdminMailSubject"];
        public override string MailSenderAddress => this.configuration["MailSenderAddress"];

        // Log Settings from file
        public override string EventLogName => this.configuration["EventLogName"];
        public override string LinuxTextLogName => this.configuration["LinuxTextLogName"];

        // SFTP Settings from file
        public override string SftpServer => this.configuration["SftpServer"];
        public override string SftpUserName => this.configuration["SftpUserName"];
        public override string SftpPassword => this.configuration["SftpPassword"];
        public override string SftpRemotePath => this.configuration["SftpRemotePath"];
        public override string SftpUploadFileName => string.Format(this.configuration["SftpUploadFileName"], dateTime.ToString("yyyyMMddHHmmss"));

        // Other Settings
        private ProcessType _processType { get; set; }
        private ImportType _importType { get; set; }
        public override ProcessType processType { get => this._processType; set => this._processType = value; }
        public override ImportType importType { get => this._importType; set => this._importType = value; }
        private bool _verbose { get; set; }
        public override bool verbose { get => this._verbose; set => this._verbose = value; }    

        public AppConfig(string[] args) : base(args)
        {

        }

    }
}
