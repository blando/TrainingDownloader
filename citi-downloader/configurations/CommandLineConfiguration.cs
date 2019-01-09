using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrainingDownloader.exceptions;
using Microsoft.Extensions.Configuration;

namespace TrainingDownloader.configurations
{
    public class CommandLineConfiguration
    {
        public enum ImportType { Full = 1, Incremental = 2 };
        public enum ProcessType { Upload = 1, Insert = 2 };
        public enum ApplicationType { Citi = 1, Aalas = 2 }

        protected IConfiguration configuration { get; set; }
        public ApplicationConfiguration applicationConfiguration { get; set; }

        public CommandLineConfiguration(string[] args)
        {
            List<string> arguments = new List<string>(args);

            string settingsFile = arguments.Where(s => s.Contains(".json")).FirstOrDefault();

            if (settingsFile == null)
            {
                throw new ParameterException("Missing JSON configuration file parameter");
            }


            this.configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
                .Build();

            applicationConfiguration = this.configuration.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>();


            if (arguments.Contains("full"))
            {
                applicationConfiguration.importType = ImportType.Full;
            }
            else if (arguments.Contains("delta"))
            {
                applicationConfiguration.importType = ImportType.Incremental;
            }
            else
            {
                throw new ParameterException("Missing import parameter (full/delta)");
            }

            if (arguments.Contains("upload"))
            {
                applicationConfiguration.processType = ProcessType.Upload;
            }
            else if (arguments.Contains("insert"))
            {
                applicationConfiguration.processType = ProcessType.Insert;
            }
            else
            {
                throw new ParameterException("Missing process type parameter (upload/insert");
            }

            if (arguments.Contains("verbose"))
            {
                applicationConfiguration.verbose = true;
            }

            switch (applicationConfiguration.VendorApplication.ToLower())
            {
                case "citi":
                    applicationConfiguration.applicationType = ApplicationType.Citi;
                    break;
                case "aalas":
                    applicationConfiguration.applicationType = ApplicationType.Aalas;
                    break;
                default:
                    throw new ParameterException("Invalid or missing VendorApplication setting");
            }

            applicationConfiguration.dateTime = DateTime.Now;

        }
    }
}
