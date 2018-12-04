using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CitiDownloader.wrappers
{
    public class AppConfig : ApplicationConfiguration
    {
        public override string FullFile => this.configuration["FullFile"];

        public override string IncrementalFile => this.configuration["IncrementalFile"];

        public override string SavePath => this.configuration["SavePath"];

        public override string SftpServer => this.configuration["SftpServer"];

        public override string SftpUserName => this.configuration["SftpUserName"];

        public override string SftpPassword => this.configuration["SftpPassword"];

        public override string SftpRemotePath => this.configuration["SftpRemotePath"];

        public override string SftpUploadFileName => this.configuration["SftpUploadFileName"];

        public override string OutputFileName => this.configuration["OutputFileName"];

        public override string FullOutputPath => this.configuration["FullOutputPath"];

        public override string FullFileName => this.configuration["FullFileName"];

        public override string IncrementalFileName => this.configuration["IncrementalFileName"];

        public override string FullSavePath => this.configuration["FullSavePath"];

        public override string IncrementalSavePath => this.configuration["IncrementalSavePath"];

        protected override IConfiguration configuration { get; set; }
        protected override DateTime dateTime {
            get => this._dateTime;
            set => this._dateTime = value;
        }
        private DateTime _dateTime { get; set; }

        public AppConfig()
        {
            this.configuration = new ConfigurationBuilder()
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .Build();
            dateTime = DateTime.Now;
        }
    }
}
